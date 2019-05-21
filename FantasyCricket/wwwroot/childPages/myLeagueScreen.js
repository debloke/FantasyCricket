"use strict";
var myLeagueScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateLeagueData( id );
    };
};

function populateLeagueData(id) {
    let myLeagueData = [
        {
            "Name": "BahutHard",
            "Members": [
                { "Player": "Debloke Maitra", "Points" : 15345 },
                { "Player": "Nakul Shrivastava", "Points" : 15445 },
                { "Player": "Pranay Ranjan", "Points" : 15120 },
                { "Player": "Saurabh Agarwal", "Points" : 15094 }
            ]
        },
        {
            "Name": "Houston Rockers",
            "Members": [
                { "Player": "Debloke Maitra", "Points" : 15345 },
                { "Player": "Nakul Shrivastava", "Points" : 15445 },
                { "Player": "Pranay Ranjan", "Points" : 15120 }
            ]
        },
        {
            "Name": "HP",
            "Members": [
                { "Player": "Gaurav Gandhi", "Points" : 11094 },
                { "Player": "Nakul Shrivastava", "Points" : 15445 },
                { "Player": "Pranay Ranjan", "Points" : 15120 }
            ]
        }
    ];
    let response = "<div id='leagueList'><ul>";
    myLeagueData.map(function(lData){
        response += "<li>"+ lData.Name +"</li>";
    });
    response += "</ul></div><div id='leagueInfo'></div>";
    $(id).html(response);
    $("#leagueList li").bind("click", function() {
        $("#leagueList li").css({"color": "#fff", "font-weight": "normal", "background": "#46e285"});
        $(this).css({"color": "#f00", "font-weight": "bold", "background": "#eef456"});
        displayLeagueInfo(this.textContent, myLeagueData);
    });
}

function displayLeagueInfo(leagueName, allData) {
    let memberData = allData.filter(function(l) { return l.Name == leagueName; });
    if(memberData && memberData.length) {
        let resp = "<table border='1'><tr><th>Rank</th><th>Member</th><th>Points</th></tr>";
        let leagueMembers = memberData[0].Members;
        leagueMembers = leagueMembers.sort(function(a,b) {
            return b.Points - a.Points;
        });
        let rank = 1;
        leagueMembers.map(function(members) {
            resp += "<tr><td>" + (rank++) + "</td><td onclick='displayPlayersTeam()'>" + members.Player + "</td><td>" + members.Points + "</td></tr>";
        });
        resp += "</table>"
        $("#leagueInfo").html(resp);        
    }
}

function displayPlayersTeam() {
    let players = [
        {"Name": "Rohit Sharma", role: "BAT", team: "India"},
        {"Name": "Shikhar Dhawan", role: "BAT", team: "India"},
        {"Name": "Virat Kohli", role: "BAT", team: "India"},
        {"Name": "Babar Azam", role: "BAT", team: "Pakistan"},
        {"Name": "MS Dhoni", role: "WK", team: "India"},
        {"Name": "Shoaib Malik", role: "ALL", team: "Pakistan"},
        {"Name": "Mohammad Hafeez", role: "ALL", team: "Pakistan"},
        {"Name": "Haris Sohail", role: "ALL", team: "Pakistan"},
        {"Name": "Hasan Ali", role: "BOWL", team: "Pakistan"},
        {"Name": "Jasprit Bumrah", role: "BOWL", team: "India"},
        {"Name": "Bhuvneshwar Kumar", role: "BOWL", team: "India"}
    ];
    circleDimensions(40, players);
    let playerPos = "";
    players.map(function(pl) {
        let coord = "'top: " + pl.yValue + "%; left: " + pl.xValue + "%;'";
        playerPos += "<span class='playerInGrid' style=" + coord +"><img class='roles' src='/icons/roles/"+pl.role+".png'/>"+ pl.Name +"</span>"
    });
    
    let displayContent = "<div class='boundary'>" + playerPos + "<div class='innerCircle'><div class='pitch'></pitch></div></div><div class='close'>X</div>";
    $(".inputContainer").html(displayContent);
    $("#inputPopup").show();
    $(".close").unbind("click");
    $(".close").bind("click", function() {
        $("#inputPopup").hide();
    });
}

function circleDimensions(radius, players){
    let coordinates = [];
    let lengthOfPlayers = players.length;
    for (let i = 0; i < lengthOfPlayers; i++) {
        players[i].xValue = (44 + radius * Math.cos(Math.PI * i / lengthOfPlayers*2-Math.PI/2));
        players[i].yValue = (48 + radius * Math.sin(Math.PI * i / lengthOfPlayers*2-Math.PI/2));
    }
}