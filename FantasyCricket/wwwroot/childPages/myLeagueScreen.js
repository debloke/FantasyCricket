"use strict";
var myLeagueScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateLeagueData( id );
    };
};

function populateLeagueData(id) {   
    let utility = new UtilityClass();
    utility.getRequest(
        "/api/score/points",
        function( data ) {
            let response = "<div id='leagueList'><ul>";
            // Only one entry as of now, Gangs will fit in here
            let myLeagueData = [
                {
                    "Name": "Overall",
                    "Members": data
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
        },
        function(err) {
            alert( "Unable to fetch Leaderboard data" );
        }
    );
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
        leagueMembers.map(function(members) {
            resp += "<tr><td>" + (rank++) + "</td><td onclick='displayPlayersTeam(\""+ members.Username +"\")'>" + members.DisplayName + "</td><td>" + members.Total + "</td></tr>";
        });
        resp += "</table>"
        $("#leagueInfo").html(resp);        
    }
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