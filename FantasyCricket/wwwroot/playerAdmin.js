"use strict";
var ALL_PLAYERS = [];
 /****************************************************************************
 * @function       Page Load                                                 *
 * @brief          Triggeres when page is loaded                             *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
$(function() {
    $.ajax({
        url: "http://localhost:5000/api/player/?tournament=ODI"
    })
    .done(function( data ) {
        ALL_PLAYERS = data;
        processData();
    });
});    
   
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
        response += "<td><img class='allCountries "+ player.teamName.replace(/ /gi, "") +"'/><span>" + player.teamName + "</span></td>";
        response += "<td>" + player.role + "</td>";
        response += "<td>" + player.cost + "</td>";
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
    let editPlayer = {
        "Name":"",
        "team":"",
        "role":"",
        "price":""
    };
    let playerId = 0;
    ALL_PLAYERS.map(function(player) {
        playerId = id || (Math.max(playerId, player.pid) + 1);
        if(id == player.pid) editPlayer = player;
    });
    createStructure(editPlayer);
    let displayContent = "<div class='editPopup'><table>";
    displayContent += "<tr><td>Player Id</td><td>"+ playerId +"</td></tr>";
    displayContent += "<tr><td>Player Name</td><td>"+ editPlayer.name +"</td></tr>";
    displayContent += "<tr><td>Player Team</td><td>"+ editPlayer.teamName +"</td></tr>";
    displayContent += "<tr><td>Player Role</td><td>"+ editPlayer.role +"</td></tr>";
    displayContent += "<tr><td>Player Price</td><td>"+ editPlayer.cost +"</td></tr>";
    displayContent += "<tr><td></td><td><button class='submitPlayer'>Submit</button></td></tr>";
    displayContent += "</table></div><div class='close'>X</div>";
    $(".inputContainerAdmin").html(displayContent);
    $("#inputPopup").show();   
    $(".close").unbind("click");
    $(".close").bind("click", function() {
        $("#inputPopup").hide();
    });
    $(".submitPlayer").bind("click", function() {
        let url = ["http://localhost:5000/api/player"];
        url.push(playerId);
        url.push($(".editPlayerData.cost").val());
        url.push($(".editPlayerData.role").val());
        url.push("?tournament=ODI");
        // AJAX call
        $.ajax({
            type: "PUT",
            url: url.join("/")
        })
        .done(function( data ) {
            location.reload(true);
        }).fail(function(err) {
            alert( "error : " + err );
        });
        $("#inputPopup").hide();
    });
}

function createStructure(obj) {
    for(let key in obj) {
        switch(key) {
            case "role" :
            case "cost" :
                obj[key] = "<input type='text' class='editPlayerData "+ key +"' value='" + obj[key] + "'/>";
                break;
            default :
                break;
        }
    }
}