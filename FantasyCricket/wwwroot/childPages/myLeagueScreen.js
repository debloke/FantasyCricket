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
            if (localStorage.loggedInUser === members.Username) {
                resp += "<td></td></tr>";
            }
            else {
                resp += "<td onclick='compareWithMyScores(\"" + members.Username + "\")'>Compare</td></tr>";
            }
            
        });
        resp += "</table>";
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
    let tableData = "<div class='comparisionTable'><div class='showParent'>";
    tableData += "<div class='show0 showI'>Table</div>";
    tableData += "<div class='show1 showI'>Chart</div>";
    tableData += "<div class='show2 showI'>Aggregate</div></div>";
    tableData += "<div class='comparisionItem'><table id='flip0' class='showP' width='100%'><thead><tr><th>Match</th>";
    ALL_MATCHES.map(function (match) {
        tempMatchData[match.unique_id] = match;
        tempMatchData[match.unique_id].game = match["team-1"] + " VS " + match["team-2"];
    });
    let pointsForChart = {};
    let sumOfPointsForChart = {};
    for (let mKey in dataToPlot) {
        let sum = 0;
        pointsForChart[mKey] = { type: "spline", showInLegend: true, name: mKey, dataPoints: [] };
        sumOfPointsForChart[mKey] = { type: "spline", showInLegend: true, name: mKey, dataPoints: [{ y: 0 }] };
        tableData += "<th>" + mKey + "</th>";
        dataToPlot[mKey].map(function (mD) {
            tempData[mD.MatchId] = tempData[mD.MatchId] || {};
            if (mKey == localStorage.loggedInUser) {
                tempData[mD.MatchId].myPoints = mD.Points;
            }
            else {
                tempData[mD.MatchId].otherPoints = mD.Points;
            }
            sum += mD.Points;
            
            pointsForChart[mKey].dataPoints.push({ label: tempMatchData[mD.MatchId].game, y: mD.Points });
            sumOfPointsForChart[mKey].dataPoints.push({ label: tempMatchData[mD.MatchId].game, y: sum });                   
        });
    }
    tableData += "</tr></thead><tbody>";

    for (let key in tempData) {
        tableData += "<tr><td>" + tempMatchData[key].game + "</td><td>" + tempData[key].myPoints + "</td><td>" + tempData[key].otherPoints + "</td></tr>";
    }
    tableData += "</tbody></table>";
    tableData += "<div id='flip1' class='flip showP'></div>";
    tableData += "<div id='flip2' class='flip showP'></div>";
    tableData += "</div></div><div class='close'>X</div>";
    $(".inputContainer").html(tableData).addClass("comaparisionItems");
    $("#inputPopup").show();

    // Draw chart for Individual match scores
    drawChart("flip1", "Points Comparision", "Matches", "Score per Game", pointsForChart);
    // Draw chart for Individual match scores
    drawChart("flip2", "Net Score Comparision", "Matches", "Total Score", sumOfPointsForChart);
 
    $(".close").unbind("click");
    $(".close").bind("click", function () {
        $("#inputPopup").hide();
    });
    $(".show0").unbind("click");
    $(".show1").unbind("click");
    $(".flip1").hide();
    $(".show0").bind("click", function () {
        $(".showP").hide();
        $(".showI").css("background", "#a56b2f");
        $("#flip0").show();
        $(this).css("background", "#ecda96");
    });
    $(".show1").bind("click", function () {
        $(".showP").hide();
        $(".showI").css("background", "#a56b2f");
        $("#flip1").show();
        $(this).css("background", "#ecda96");
    });
    $(".show2").bind("click", function () {
        $(".showP").hide();
        $(".showI").css("background", "#a56b2f");
        $("#flip2").show();
        $(this).css("background", "#ecda96");
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
            $(".inputContainer").html(displayContent + centralData).removeClass("comaparisionItems");
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

function drawChart(uiId, title, xLabel, yLabel, data) {
    let chart = new CanvasJS.Chart(uiId, {
        animationEnabled: true,
        exportEnabled: true,
        theme: "light2",
        title: { text: title },
        axisY: { title: yLabel, titleFontSize: 24 },
        axisX: { title: xLabel,
                 titleFontSize: 24,
                 labelFormatter: function(e){
				    return  "";
			     }
        },
        data: Object.values(data)
    });
    chart.render();
}