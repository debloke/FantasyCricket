"use strict";
var liveScoreScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateLiveData( id );
    };
};

function populateLiveData(id) {
    let oldData = [];
    let newData = [];
    let utility = new UtilityClass();
    // AJAX call : HARDCODED FOR NOW
    // TO BE CHANGED WITH LIVE GAME ID
    utility.getRequest(
        "/api/score/1152843",
        function( data ) {
            oldData = data;
            newData = data;
            plotDataToUI(oldData, newData, id);
        },
        function(err) {
            alert( "Unable to fetch Live data" );
        }
    );
}

function plotDataToUI(oData, nData, id) {
    let player = {};
    let playerPerTeam = {};
    oData.map(function(opData) {
        player[opData.pid] = opData;
        player[opData.pid].Points = opData.BattingPoints + opData.FieldingPoints + opData.BowlingPoints;
    });
    nData.map(function(npData) {
        playerPerTeam[npData.Team] = playerPerTeam[npData.Team] || [];
        player[npData.pid].NewBattingPoints = npData.BattingPoints;
        player[npData.pid].NewFieldingPoints = npData.FieldingPoints;
        player[npData.pid].NewBowlingPoints = npData.BowlingPoints;
        player[npData.pid].NewPoints = npData.BattingPoints + npData.FieldingPoints + npData.BowlingPoints;
        playerPerTeam[npData.Team].push(player[npData.pid]);
    });
    drawData(playerPerTeam, id);
}

function drawData(data, id) {
    let response = "";
    let teamList = [];

    function determineIcon(oldPoints, newPoints) {
        return (newPoints > oldPoints) ?
               "<img style='margin: 0 0 0 5px; width:12px;' src='/icons/up.png'/>" :
               ((oldPoints > newPoints) ? "<img style='margin: 0 0 0 5px; width:12px;' src='/icons/down.png'/>" : "");
    }

    for(let key in data) {
        teamList.push(key);
        let teamData = data[key];
        let tData = "<tr><td>Team</td><td>Player</td><td>Batting</td><td>Fielding</td><td>Bowling</td><td>Total</td></tr>";
        teamData.map(function(pData) {
            let rowData = "";
            rowData += "<td><img class='allCountries "+ pData.Team.replace(/ /gi, "") +"'/></td>";
            rowData += "<td>"+ pData.name +"</td>";
            rowData += "<td>"+ pData.NewBattingPoints + determineIcon(pData.BattingPoints, pData.NewBattingPoints) + "</td>";
            rowData += "<td>"+ pData.NewFieldingPoints + determineIcon(pData.FieldingPoints, pData.NewFieldingPoints) + "</td>";
            rowData += "<td>"+ pData.NewBowlingPoints + determineIcon(pData.BowlingPoints, pData.NewBowlingPoints) + "</td>";
            rowData += "<td>"+ pData.NewPoints + determineIcon(pData.Points, pData.NewPoints) + "</td>";
            tData += "<tr>"+ rowData +"</tr>"
        });
        response += "<td style='vertical-align: top;'><table style='display:inline;'>" + tData + "</table></td>";
    }
    $(id).html( "<div align='center' class='liveData'><table border='1'><tr><th colspan='2'>" +
        teamList.join(" VS ") +
        "</th></tr>" +
        "<tr>" + response + "</tr>" +
        "</table></div>");
}