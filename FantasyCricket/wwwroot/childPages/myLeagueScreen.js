"use strict";
var myLeagueScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateLeagueData( id );
    };
};

function createLeagueData(id) {
    let response = [];
    response.push({ "Name": "Overall", "Members": ALL_POINTS });
    let utility = new UtilityClass();
    utility.getRequest(
        "/api/gangs?seriesId=" + localStorage.seriesId,
        function (data) {
            ALL_MY_GROUPS = data;
            let tempData = {};
            let gangs = {};
            ALL_POINTS.map(function (usr) {
                tempData[usr.Username] = usr;
            });
            ALL_MY_GROUPS.map(function (gp) {
                gangs[gp.GangId] = gangs[gp.GangId] || { "Name": gp.GangName, "Members": [], "GangId": gp.GangId, "GangOwner": gp.GangOwner };
                gangs[gp.GangId].Members.push(tempData[gp.Username] || { "Username": gp.Username, "Total": 0, "DisplayName": gp.Username });
                if(localStorage.loggedInUser === gp.Username)  gangs[gp.GangId].Approved = gp.Approved;
            });
            response = response.concat(Object.values(gangs));
            displayData(response, id, utility);
        },
        function (err) {
            alert("Unable to fetch players data for " + url);
        }
    );
}

function populateLeagueData(id) {
    createLeagueData(id);
}

function displayData(myLeagueData, id, utility) {
    let response = "<div id='leagueList'><ul>";
    myLeagueData.map(function(lData){
        if(lData.Name == "Overall") {
            response += "<li style='border-top:4px solid #5554a2'>"+ lData.Name +"</li>";
        }
        else {
            response += "<li gangId='" + lData.GangId +"'>"+ lData.Name +"</li>";
        }
    });
    response += "</ul><input type='text' id='addGang'/><button id='addGangBtn'>Create</button></li>";
    response += "</div><div id='leagueInfo'></div>";
    $(id).html(response);
    displayLeagueInfo(myLeagueData);
    $("#leagueList li").bind("click", function() {
        $("#leagueList li").css({"border-top": "none"});
        $(this).css({"border-top": "4px solid #5554a2"});
        displayLeagueInfo(myLeagueData, id, this.getAttribute("gangid"), utility);
    });

    $("#addGangBtn").bind("click", function () {
        utility.postRequest(
            "/api/gangs",
            function (data) {
                createLeagueData(id);
            },
            function (err) {
                alert("Error adding gang");
            },
            { "GangName": $("#addGang").val(), "SeriesId": localStorage.seriesId }
        );
    });
}

function displayLeagueInfo(allData, id, gangid, utility) {
    let memberData = allData.filter(function(l) { return l.GangId == gangid; });
    if(memberData && memberData.length) {
        if(memberData[0].Approved != 0) {
            let resp = "<table border='1'><tr><th>Rank</th><th>Member</th><th>Points</th></tr>";
            let leagueMembers = memberData[0].Members;
            leagueMembers = leagueMembers.sort(function(a,b) {
                return b.Total - a.Total;
            });
            let rank = 1;
            let presentMembers = [];
            leagueMembers.map(function (members) {
                resp += "<tr><td>" + (rank++) + "</td>";
                resp += "<td onclick='displayPlayersTeam(\"" + members.Username + "\")'>" + members.DisplayName + "</td>";
                resp += "<td>" + members.Total + "</td>";
                if (localStorage.loggedInUser === members.Username) {
                    resp += "<td onclick='compareWithMyScores()'>History</td><td></td></tr>";
                }
                else if(memberData[0].GangOwner && (localStorage.loggedInUser === memberData[0].GangOwner)) {
                    resp += "<td onclick='compareWithMyScores(\"" + members.Username + "\")'>Compare</td><td class='removeMemberFromGang' data='"+ members.Username +"'>Remove</td></tr>";
                }
                else {
                    resp += "<td onclick='compareWithMyScores(\"" + members.Username + "\")'>Compare</td><td></td></tr>";
                }
                presentMembers.push(members.Username);     
            });
            resp += "</table>";
            if (gangid && memberData[0].GangOwner == localStorage.loggedInUser) {
                resp += "<button class='deleteGang'>Delete Gang</button>";
                resp += "<div style='margin-top: 10px'><span>Add User: </span><input type='text' class='searchUser'/></div>";
                resp += "<table id='userList'><table>";
            }

            $("#leagueInfo").html(resp);

            $(".deleteGang").bind("click", function () {
                utility.deleteRequest(
                    "/api/gangs/" + gangid,
                    function () { createLeagueData(id) },
                    function () { alert("Cannot delete gang"); }
                );
            });

            $(".removeMemberFromGang").bind("click", function() {
                let user = this.getAttribute("data");
                utility.deleteRequest(
                    "/api/gangs/" + gangid + "/removeuser",
                    function () { createLeagueData(id) },
                    function () { alert("Cannot delete user"); },
                    [user]
                );
            });

            $(".searchUser").bind("keyup", function () {
                let resp = "";
                let absentMembers = ALL_POINTS.filter(function(m) {
                    return (presentMembers.indexOf(m.Username) === -1) &&
                        ((m.Username.toLowerCase().indexOf($(".searchUser").val().toLowerCase()) > -1) ||
                         (m.DisplayName.toLowerCase().indexOf($(".searchUser").val().toLowerCase()) > -1));
                });
                absentMembers.map(function(am) {
                    resp += "<tr><td>"+am.Username+"<td><td>"+am.DisplayName+"<td><td data='"+ am.Username +"' class='addMemberToGrp'>+</td></tr>"
                });
                $("#userList").html(resp);

                $(".addMemberToGrp").unbind("click");
                $(".addMemberToGrp").bind("click", function() {
                    utility.postRequest(
                        "/api/gangs/" + gangid,
                        function() { createLeagueData(id); },
                        function() { alert("Error adding member"); },
                        [this.getAttribute("data")]
                    );
                });
            });
        }
        else {
            let resp = "<button class='approveJoiningGang'>Join Gang</button>"
            $("#leagueInfo").html(resp);
            $(".approveJoiningGang").unbind("click");
            $(".approveJoiningGang").bind("click", function() {
                utility.putRequest(
                    "/api/gangs/" + gangid,
                    function () { createLeagueData(id) },
                    function () { alert("Cannot approve joining group"); }
                );
            });
        }
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
    if (player) listOfPromises.push(createPromise("/api/user/team/history?username=" + player));

    Promise.all(listOfPromises).then(function (values) {
        let comparisionObj = {};
        comparisionObj[localStorage.loggedInUser] = values[0];
        if (values[1]) comparisionObj[player] = values[1];
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
    tableData += "<div class='comparisionItem'><table id='flip0' class='showP selected' width='100%'><thead><tr><th>Match</th>";
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
        tableData += "<tr>";
        tableData += "<td>" + tempMatchData[key].game + "</td>";
        if (!isNaN(tempData[key].myPoints)) tableData += "<td>" + tempData[key].myPoints + "</td>";
        if (!isNaN(tempData[key].otherPoints)) tableData += "<td>" + tempData[key].otherPoints + "</td>";
        tableData += "</tr>";
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
        $(".showI").removeClass("selected");
        $("#flip0").show();
        $(this).addClass("selected");
    });
    $(".show1").bind("click", function () {
        $(".showP").hide();
        $(".showI").removeClass("selected");
        $("#flip1").show();
        $(this).addClass("selected");
    });
    $(".show2").bind("click", function () {
        $(".showP").hide();
        $(".showI").removeClass("selected");
        $("#flip2").show();
        $(this).addClass("selected");
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