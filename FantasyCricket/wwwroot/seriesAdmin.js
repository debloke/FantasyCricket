"use strict";
var ALL_PLAYERS = [];
 /****************************************************************************
 * @function       Page Load                                                 *
 * @brief          Triggeres when page is loaded                             *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
$(function() {
    let utility = new UtilityClass();
    
    // Promise to fetch Series info
    let seriesPromise = new Promise(function(resolve, reject) {
        utility.getRequest(
            "/api/series",
            function( data ) {
                resolve(data);
            },
            function(err) {
                alert( "Unable to fetch Live data" );
            }
        );
    });
    
    // Promise to fetch Series info
    let matchesPromise = new Promise(function(resolve, reject) {
        utility.getRequest(
            "/api/series/matches",
            function( data ) {
                resolve(data);
            },
            function(err) {
                alert( "Unable to fetch Live data" );
            }
        );
    });

    let tempData = {};
    Promise.all([seriesPromise, matchesPromise]).then(function(values) {
        let seriesData = values[0];
        let matchData = values[1];
        let objSeries = {};
        seriesData.map(function(sD) {
            tempData[sD.SeriesId] = {
                "SeriesName" : sD.SeriesName,
                "Type" : "ODI",
                "Matches" : []
            };
        });
        matchData.map(function(mD) {
            tempData[mD.SeriesId].Mathes.push({
                "Date" : mD.dateTimeGMT,
                "Team1" : md["team-1"],
                "Team2" : md["team-1"]
            });
        });
        
    });
    
    let seriesData = Object.values(tempData);
    processData(seriesData);
});

 /****************************************************************************
 * @function       processData                                               *
 * @brief          Process data once we have it from http call               *
 * @param          {Object} data - Data to be rendered                       *
 * @returns        N/A                                                       *
 ****************************************************************************/
function processData(data) {
    let header = "<span id='AddSeries'>+</span>";
    let seriesTitleContent = "<ul>";
    data.map(function(sData) {
        seriesTitleContent += "<li>" + sData.SeriesName + "</li>";
    });
    seriesTitleContent += "</ul>";
    $(".seriesLeftBlock").html(header + seriesTitleContent);
    
    $("#AddSeries").unbind("click");
    $("#AddSeries").bind("click", function() {
        openPopupAndCreateRecord(data);
    });
    $(".seriesLeftBlock ul li").unbind("click");
    $(".seriesLeftBlock ul li").bind("click", function() {
        $( ".seriesLeftBlock ul li" ).removeClass("selected");
        $(this).addClass("selected");
        let selectedSeries = this.innerText;
        data.map(function(mData){
            if(mData.SeriesName == selectedSeries ) {
                displayInRightContainer(mData);
            }
        });
    });
}

function displayInRightContainer(seriesDesc) {
    let header = "<h3>"+ seriesDesc.SeriesName +"</h3>"
    let seriesMatchesContent = "<table><tr><th>Date</th><th>Team 1</th><th>Team 2</th></tr>";
    if( seriesDesc.Matches.length ) {
        seriesDesc.Matches.map(function(matches) {
            seriesMatchesContent += "<tr>";
            seriesMatchesContent += "<td>"+ matches.Date +"</td>";
            seriesMatchesContent += "<td><img class='allCountries "+ matches.Team1.replace(/ /gi, "") +"'/><span>" + matches.Team1 + "</span></td>";
            seriesMatchesContent += "<td><img class='allCountries "+ matches.Team2.replace(/ /gi, "") +"'/><span>" + matches.Team2 + "</span></td>";
            seriesMatchesContent += "</tr>";
        });
    }
    seriesMatchesContent += "</table>";
    $(".seriesDescription").html(header + seriesMatchesContent);
}

function openPopupAndCreateRecord(data) {
    $("#inputPopup").show();
    $(".close").unbind("click");
    $("#addSeries").unbind("click");
    $("#addSeries").bind("click", function() {
        let obj = {};
        obj.SeriesName = $("#serName").val();
        obj.Type = $("#serType").val();
        obj.Matches = JSON.parse($("#matches").val());
        // Clear all data
        $("#serName").val("");
        $("#serType").val("");
        $("#matches").val("");
        data.push(obj);
        $("#inputPopup").hide();
        processData(data);
    });
    $(".close").bind("click", function() {
        $("#inputPopup").hide();
    });
}