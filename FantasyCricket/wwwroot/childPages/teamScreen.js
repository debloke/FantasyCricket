"use strict";
let ALL_PLAYERS = [];
var teamScreen = function() {
    this.returnDataForDisplay = function(id) {
        let utility = new UtilityClass();
        let listOfPromises = [];
        function createPromise(url) {
            return new Promise(function(resolve, reject) {
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
            
        // Promise to fetch all players info
        listOfPromises.push(createPromise("/api/player/?tournament=ODI"));
        // Promise to fetch my players info
        listOfPromises.push(createPromise("/api/user/team/?magic=" + sessionStorage.guid));

        Promise.all(listOfPromises).then(function (values) {
            ALL_PLAYERS = values[0];       
            populateTeamData(id, values[1]);
        });
    };
};

function populateTeamData(id, listOfMyPlayers) {
    let myPlayers = listOfMyPlayers.PlayerIds || [];
    let myPlayersFullDetail = [];
    let listOfRoleFilters = ["FULL"];
    let listOfTeamFilters = ["FULL"];
    let filter = { Role: "FULL", TeamName: "FULL" };
    ALL_PLAYERS.map(function(player) {
        if(listOfRoleFilters.indexOf(player.Role) == -1) listOfRoleFilters.push(player.Role);
        if(listOfTeamFilters.indexOf(player.TeamName) == -1) listOfTeamFilters.push(player.TeamName);
    });

    function displayPlayerList() {
        let list = "";    
        ALL_PLAYERS.map(function(player) {
            if( myPlayers.indexOf(player.pid) == -1 ) {
                if(((filter.Role == "FULL") || (filter.Role == player.Role)) && ((filter.TeamName == "FULL") || (filter.TeamName == player.TeamName))) {
                    list+= "<li class='addPlayer' data='"+player.pid+"'>";
                    list+= "<span class='playerName'>"+player.name+"</span>";
                    list+= "<span class='playerTeam'>"+player.TeamName+"</span>";
                    list+= "<span class='playerPrice'>"+player.Cost+"</span>";
                }
            }
        });
        return list;
    }

    function fillDataInUI() {
        let leftColData = "";
        let centerColData = "";
        let rightColData = "";
        centerColData = displayPlayerList();
        listOfRoleFilters.map(function(role){
            leftColData += (role == filter.Role) ? "<li class='selected'>"+role+"</li>" : "<li>"+role+"</li>";
        });
        listOfTeamFilters.map(function(team){
            rightColData += (team == filter.TeamName) ? "<li class='selected'>"+team+"</li>" : "<li>"+team+"</li>";
        });
        myPlayersFullDetail = [];
        let myBatsmanData = "";
        let myBowlerData = "";
        let myAllRounderData = "";
        let myWicketKeeperData = "";
        let captains = "";
        ALL_PLAYERS.map(function(player){
            if(myPlayers.indexOf(player.pid) > -1) {
                if(player.pid == listOfMyPlayers.BattingCaptain) {
                    captains += "<div>Batting Captain: " + player.name +"</div>";
                }
                if(player.pid == listOfMyPlayers.BowlingCaptain) {
                    captains += "<div>Bowling Captain: " + player.name +"</div>";
                }
                if(player.pid == listOfMyPlayers.FieldingCaptain) {
                    captains += "<div>Fielding Captain: " + player.name +"</div>";
                }
                
                myPlayersFullDetail.push(player);
                let mPDate = "<li>";
                player.name = (player.name.length > 13) ? (player.name.substring(0,13) + "..") : player.name;
                mPDate += "<div class='iconContainer'><img class='allCountries "+player.TeamName.replace(/ /gi, "")+"'/>";
                mPDate += "<img class='roles' src='/icons/roles/"+player.Role+".png'/></div>";
                mPDate += "<div class='playerInfo'><span class='pName'>"+player.name+"</span>";
                mPDate += "<span class='pPrice'>"+player.Cost+"</span></div>";
                mPDate += "<img src='/icons/delete.png' class='deletePlayer' data='"+player.pid+"'></li>";

                switch(player.Role) {
                    case "BAT": myBatsmanData+= mPDate; break;
                    case "WK": myWicketKeeperData+= mPDate; break;
                    case "ALL": myAllRounderData+= mPDate; break;
                    case "BOWL": myBowlerData+= mPDate; break;
                    default: break;
                }
            }
        });
        let uiData = "<div class='myTeam'>";
        uiData += "<p id='errorMessage'></p>";
        uiData += "<span id='saveteam'>Submit</span>";
        uiData += "<p id='budgetLeftMessage'></p>";
        uiData += "<p id='subsLeftMessage'></p>";
        uiData += "<div><ul>"+myBatsmanData+"</ul></div>";
        uiData += "<div><ul>"+myWicketKeeperData+"</ul></div>";
        uiData += "<div><ul>"+myAllRounderData+"</ul></div>";
        uiData += "<div><ul>"+myBowlerData+"</ul></div></div>";

        let innerData = "<div class='leftTeamItems'><ul>"+leftColData+"</ul></div>";
        innerData += "<div class='centerTeamItems'><ul>"+centerColData+"</ul></div>";
        innerData += "<div class='rightTeamItems'><ul>"+rightColData+"</ul></div>";
        uiData += "<div class='teamList'>"+innerData+"</div>";
        $(id).html(uiData);

        let teamValidation = new validator(myPlayersFullDetail);
        let response = teamValidation.runValidations();
        (response.isError) && $("#errorMessage").html(response.error);
        $("#budgetLeftMessage").html("Budget Left: " + response.budgetLeft + captains);
        $("#subsLeftMessage").html("Substitutions Left: " + (myPlayers.length ? listOfMyPlayers.RemSubs : "&#8734;"));
        $("#saveteam").css("visibility", ( response.isError ? "hidden" : "visible" ));
        
        // Save Team
        $("#saveteam").bind("click", function() {
            openPopupForCaptains(listOfMyPlayers, myPlayersFullDetail);
        });

        $( ".leftTeamItems li" ).unbind( "click" );
        $( ".leftTeamItems li" ).bind("click", function() {
            $( ".leftTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.Role = $(this)[0].innerText;
            fillDataInUI();
        });
        $( ".rightTeamItems li" ).unbind( "click" );
        $( ".rightTeamItems li" ).bind("click", function() {
            $( ".rightTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.TeamName = $(this)[0].innerText;
            fillDataInUI();
        });
        $( ".addPlayer" ).unbind( "click" );
        $( ".addPlayer" ).bind( "click", function() {
            fillInMyList(+this.getAttribute("data"))
        });
        $( ".deletePlayer" ).unbind( "click" );
        $( ".deletePlayer" ).bind( "click", function() {
            let idToRemove = +this.getAttribute("data");
            let deleteIndex = myPlayers.indexOf(idToRemove);
            myPlayers.splice(deleteIndex,1);
            fillDataInUI();
        });
    }

    function fillInMyList(id) {
        myPlayers.push(id);
        fillDataInUI();
    }

    fillDataInUI();
}

function openPopupForCaptains(listOfMyPlayers, myPlayersFullDetail) {
    let selectBattingCaptain = "Batting Captain: <select id='myBatCaptain'><option value='0'>Select</option>";
    let selectBowlingCaptain = "Bowling Captain: <select id='myBowlCaptain'><option value='0'>Select</option>";
    let selectFieldingCaptain = "Fielding Captain: <select id='myFieldCaptain'><option value='0'>Select</option>";
    listOfMyPlayers.PlayerIds = [];
    myPlayersFullDetail.map(function(player) {
        listOfMyPlayers.PlayerIds.push(player.pid);
        selectBattingCaptain += "<option value='"+ player.pid +"'>"+ player.name +"</option>";
        selectBowlingCaptain += "<option value='"+ player.pid +"'>"+ player.name +"</option>";
        selectFieldingCaptain += "<option value='"+ player.pid +"'>"+ player.name +"</option>";
    });
    selectBattingCaptain += "</select><br/>";
    selectBowlingCaptain += "</select><br/>";
    selectFieldingCaptain += "</select><br/>";
    let submitBtn = "<button id='saveMyTeam'>Save</button>";
        
    $(".inputContainer").html(selectBattingCaptain + selectBowlingCaptain + selectFieldingCaptain + submitBtn).css({"color":"white", "background":"#8a3737", "width":"40%", "height":"auto", "left":"30%", "border-radius": "10px"});
    
    $("#inputPopup").css({"z-index": 20}).show();
    
    $(".glassBackground").unbind("click");
    $("#saveMyTeam").unbind("click");
    $(".glassBackground").bind("click", function() {
        $("#inputPopup").hide();
    });
    
    $("#saveMyTeam").bind("click", function() {
        listOfMyPlayers.BattingCaptain = +$("#myBatCaptain").val();
        listOfMyPlayers.BowlingCaptain = +$("#myBowlCaptain").val();
        listOfMyPlayers.FieldingCaptain = +$("#myFieldCaptain").val();
        if(!listOfMyPlayers.BattingCaptain || !listOfMyPlayers.BowlingCaptain || !listOfMyPlayers.FieldingCaptain) {
            alert("Select all 3 captains");
        }
        else {
            let utility = new UtilityClass();
            utility.postRequest(
                "/api/user/team/?magic=" + sessionStorage.guid,
                function (data) {
                    window.location.reload(true);
                },
                function (err) {
                    alert(err);
                },
                listOfMyPlayers
            );
        }
    });      
}