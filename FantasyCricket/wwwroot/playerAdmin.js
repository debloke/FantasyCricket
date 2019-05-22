"use strict";
var ALL_PLAYERS = [];
 /****************************************************************************
 * @function       Page Load                                                 *
 * @brief          Triggeres when page is loaded                             *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
$(function() {
    pollData();
});

function pollData() {
    let utility = new UtilityClass();
    // AJAX call
    utility.getRequest(
        "/api/player/?tournament=ODI",
        function( data ) {
            ALL_PLAYERS = data.sort(function(a,b) {
                let val1 = a.TeamName;
                let val2 = b.TeamName;
                if (val1 == val2) return 0;
                if (val1 > val2) return 1;
                if (val1 < val2) return -1;
            });
            processData();
        },
        function(err) {
            alert( "Unable to fetch all players data" );
        }
    );

    // TO BE DELETED
    fetchListOfMatchesForDefinedSeries(utility);
}

 /****************************************************************************
 * @function       processData                                               *
 * @brief          Process data once we have it from http call               *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
function processData() {
    let response = "<tr><th colspan='6'><h3>Player Admin</h3></th></tr>";
    response += "<tr><th>Id</th><th>Name</th><th>Team</th><th>Role</th><th>Price</th><th>Edit</th></tr>";
    ALL_PLAYERS.map(function(player) {       
        response += "<tr>";
        response += "<td>" + player.pid + "</td>";
        response += "<td>" + player.name + "</td>";
        response += "<td><img class='allCountries "+ player.TeamName.replace(/ /gi, "") +"'/><span>" + player.TeamName + "</span></td>";
        response += "<td>" + player.Role + "</td>";
        response += "<td>" + player.Cost + "</td>";
        response += "<td><img style='width:20px;' class='editPlayerDetail' playerId='"+player.pid+"' src='/icons/edit.png'/></td>";
        response += "</tr>";
    });
    $(".playerAdmin table").html(response);
    $(".editPlayerDetail").bind("click", function() {
        let playerId = this.getAttribute("playerId");
        displayPlayerDetailForChange(playerId);
    });
}

function displayPlayerDetailForChange(id) {
    let utility = new UtilityClass();
    let editPlayer = {
        "Name":"",
        "team":"",
        "Role":"",
        "Cost":""
    };
    let playerId = 0;
    ALL_PLAYERS.map(function(player) {
        playerId = id || (Math.max(playerId, player.pid) + 1);
        if(id == player.pid) editPlayer = JSON.parse(JSON.stringify(player));
    });
    createStructure(editPlayer);
    let displayContent = "<div class='editPopup'><table>";
    displayContent += "<tr><td>Player Id</td><td>"+ playerId +"</td></tr>";
    displayContent += "<tr><td>Player Name</td><td>"+ editPlayer.name +"</td></tr>";
    displayContent += "<tr><td>Player Team</td><td>"+ editPlayer.TeamName +"</td></tr>";
    displayContent += "<tr><td>Player Role</td><td>"+ editPlayer.Role +"</td></tr>";
    displayContent += "<tr><td>Player Price</td><td>"+ editPlayer.Cost +"</td></tr>";
    displayContent += "<tr><td></td><td><button class='submitPlayer'>Submit</button></td></tr>";
    displayContent += "</table></div><div class='close'>X</div>";
    $(".inputContainerAdmin").html(displayContent);
    $("#inputPopup").show();
    $(".close").unbind("click");
    $(".close").bind("click", function() {
        $("#inputPopup").hide();
    });


    $(".submitPlayer").bind("click", function() {
        let url = ["/api/player"];
        url.push(playerId);
        url.push($(".editPlayerData.Cost")[0].value);
        url.push($(".editPlayerData.Role").val());
        url.push("?tournament=ODI");
        // AJAX call
        utility.putRequest(
            url.join("/"),
            function( data ) {
                location.reload(true);
            },
            function(err) {
                alert( "Unable to update player data" );
            }
        );
        $("#inputPopup").hide();
    });
}

function createStructure(obj) {
    for(let key in obj) {
        switch(key) {
            case "Role" : 
                obj[key] = "<select class='editPlayerData "+ key +"'>";
                obj[key] += "<option value='BAT'>BAT</option>";
                obj[key] += "<option value='BOWL'>BOWL</option>";
                obj[key] += "<option value='WK'>WK</option>";
                obj[key] += "<option value='ALL'>ALL</option>";
                obj[key] += "</select>";
                break;
            case "Cost" :
                obj[key] = "<input type='text' class='editPlayerData "+ key +"' value='" + obj[key] + "'/>";
                break;
            default :
                break;
        }
    }
}

function fetchListOfMatchesForDefinedSeries(utility) {
    utility.getRequest(
        "/api/series/matches",
        function (data) {
            let res = "<option>Select</option>";
            data.map(function(match) {
                res += " <option value='" + match.unique_id + "'>" + match["team-1"] + " VS " + match["team-2"] + "</option>"
            });
            $("#match").html(res);
        },
        function (err) {
            reject("Unable to fetch matches data");
        }
    );
}

function importPlayers() {
    let matchId = +($("#match")[0].value);
    if(matchId) {
        let utility = new UtilityClass();
        utility.putRequest(
            "/api/player/"+ matchId +"?tournament=odi",
            function(data) {
                pollData();
            },
            function(err) {
                alert("error polling data");
            }
        );
    }
}