"use strict";
var myLeagueScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateLeagueData( id );
    };
};

function populateLeagueData(id) {   
    let response = "<div id='leagueList'><ul>";
    // Only one entry as of now, Gangs will fit in here
    let myLeagueData = [
        {
            "Name": "Overall",
            "Members": ALL_POINTS
        }
    ];
    myLeagueData.map(function(lData){
        if(lData.Name == "Overall") {
            response += "<li style='border-top:4px solid #5554a2'>"+ lData.Name +"</li>";
        }
        else {
            response += "<li>"+ lData.Name +"</li>";
        }
    });
    response += "</ul></div><div id='leagueInfo'></div>";
    $(id).html(response);
    displayLeagueInfo("Overall", myLeagueData);
    $("#leagueList li").bind("click", function() {
        $("#leagueList li").css({"border-top": "none"});
        $(this).css({"border-top": "4px solid #5554a2"});
        displayLeagueInfo(this.textContent, myLeagueData);
    });
}

function displayLeagueInfo(leagueName, allData) {
    let memberData = allData.filter(function(l) { return l.Name == leagueName; });
    if(memberData && memberData.length) {
        let resp = "<table border='1'><tr><th>Rank</th><th>Member</th><th>Points</th></tr>";
        let leagueMembers = memberData[0].Members;
        leagueMembers = leagueMembers.sort(function(a,b) {
            return b.Total - a.Total;
        });
        let rank = 1;
        leagueMembers.map(function (members) {
            resp += "<tr><td>" + (rank++) + "</td>";
            resp += "<td onclick='displayPlayersTeam(\"" + members.Username + "\")'>" + members.DisplayName + "</td>";
            resp += "<td>" + members.Total + "</td>";
            resp += "<td onclick='compareWithMyScores(\"" + members.Username + "\")'>Compare</td></tr>";
        });
        resp += "</table>"
        $("#leagueInfo").html(resp);
    }
}

function compareWithMyScores(player) {
    let utility = new UtilityClass();
    let listOfPromises = [];
    function createPromise(url) {
        return new Promise(function (resolve, reject) {
            utility.getRequest(
                url,
                function (data) {
                    resolve(data);
                },
                function (err) {
                    alert("Unable to fetch players data for " + url);
                }
            );
        });
    }

    // Promise to fetch my points history
    listOfPromises.push(createPromise("/api/user/team/history?username=" + localStorage.loggedInUser));
    // Promise to fetch other player's points history
    listOfPromises.push(createPromise("/api/user/team/history?username=" + player));

    Promise.all(listOfPromises).then(function (values) {
        let comparisionObj = {};
        comparisionObj[localStorage.loggedInUser] = values[0];
        comparisionObj[player] = values[1];
        drawComparision(comparisionObj);
    });
}

function drawComparision(dataToPlot) {
    let tempData = {};
    let tempMatchData = {};
    let tableData = "<div class='comparisionTable'><div><span class='show0'>Table</span><span class='show1'>Chart</span></div>";
    tableData += "<table id='flip0' width='100%'><tr><th>Match</th>";
    ALL_MATCHES.map(function (match) {
        tempMatchData[match.unique_id] = match;
        tempMatchData[match.unique_id].game = match["team-1"] + " VS " + match["team-2"];
    });
    let pointsForChart = [];
    for (let mKey in dataToPlot) {
        pointsForChart[mKey] = {
            type: "line",
            dataPoints: []
        };
        tableData += "<th>"+ mKey +"</th>";
        dataToPlot[mKey].map(function (mD) {
            tempData[mD.MatchId] = tempData[mD.MatchId] || {};
            if (mKey == localStorage.loggedInUser) {
                tempData[mD.MatchId].myPoints = mD.Points;
            }
            else {
                tempData[mD.MatchId].otherPoints = mD.Points;
            }
            
            if (pointsForChart[mKey].dataPoints.length == 1) {
                pointsForChart[mKey].dataPoints.push({ y: mD.Points, indexLabel: mKey });
            }
            else {
                pointsForChart[mKey].dataPoints.push({ y: mD.Points });
            }

            
        });
    }
    tableData += "</tr>";

    for (let key in tempData) {
        tableData += "<tr><td>" + tempMatchData[key].game + "</td><td>" + tempData[key].myPoints + "</td><td>" + tempData[key].otherPoints + "</td></tr>";
    }
    tableData += "</table><div id='flip1' style='width:100%; height:auto;'></div></div><div class='close'>X</div>";
    $(".inputContainer").html(tableData).css({"width": "50%", "left": "25%", "background": "white"});
    $("#inputPopup").show();

    var chart = new CanvasJS.Chart("flip1", {
        animationEnabled: true,
        theme: "light2",
        title: { text: "Points Comparision" },
        axisY: { includeZero: false },
        data: Object.values(pointsForChart)
    });
    chart.render();


    $(".close").unbind("click");
    $(".close").bind("click", function () {
        $("#inputPopup").hide();
    });
    $(".show0").unbind("click");
    $(".show1").unbind("click");
    $(".flip1").hide();
    $(".show0").bind("click", function () {
        $("#flip0").show();
        $("#flip1").hide();
    });
    $(".show1").bind("click", function () {
        $("#flip0").hide();
        $("#flip1").show();
    });
}

function displayPlayersTeam(user) {
    let utility = new UtilityClass();
    utility.getRequest(
        "/api/user/team/others?username="+user,
        function( data ) {
            let players = [];
            let centralData = "";
            ALL_PLAYERS.map(function(player) {
                if(data.PlayerIds.indexOf(player.pid) > -1) {
                    players.push(player);
                }
                if(data.BattingCaptain == player.pid) centralData += "<div class='popupBatCaptain'><img class='popupRoles' src='/icons/roles/BAT.png'/>" + player.name + "</div>";
                if(data.BowlingCaptain == player.pid) centralData += "<div class='popupBowlCaptain'><img class='popupRoles' src='/icons/roles/BOWL.png'/>" + player.name + "</div>";
                if(data.FieldingCaptain == player.pid) centralData += "<div class='popupFieldCaptain'><img class='popupRoles' src='/icons/roles/WK.png'/>" + player.name + "</div>";
                
            });
            centralData += "<div class='subsPopup'>Subs Remaining : " + data.RemSubs + "</div>";
            circleDimensions(40, players);
            let playerPos = "";
            players.map(function(pl) {
                let coord = "'top: " + pl.yValue + "%; left: " + pl.xValue + "%;'";
                playerPos += "<span class='playerInGrid' style=" + coord +">";
                playerPos += "<img class='roles' src='/icons/roles/"+pl.Role+".png'/>";
                playerPos += "<img class='allCountries "+ pl.TeamName.replace(/ /gi, "") +"'/>";
                playerPos += "<p class='namePl'>"+ pl.name +" (" + pl.Cost + ")</p></span>"
            });
            
            let displayContent = "<div class='boundary'>" + playerPos + "<div class='innerCircle'><div class='pitch'></pitch></div></div><div class='close'>X</div>";
            $(".inputContainer").html(displayContent + centralData);
            $("#inputPopup").show();
            $(".close").unbind("click");
            $(".close").bind("click", function() {
                $("#inputPopup").hide();
            });
        },
        function( err ) {
            alert("Error fetching team for: " + user);
        }
    );
}

function circleDimensions(radius, players){
    let coordinates = [];
    let lengthOfPlayers = players.length;
    for (let i = 0; i < lengthOfPlayers; i++) {
        players[i].xValue = (44 + radius * Math.cos(Math.PI * i / lengthOfPlayers*2-Math.PI/2));
        players[i].yValue = (44 + radius * Math.sin(Math.PI * i / lengthOfPlayers*2-Math.PI/2));
    }
}