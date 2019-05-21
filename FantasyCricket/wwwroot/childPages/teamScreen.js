"use strict";
var teamScreen = function() {
    this.returnDataForDisplay = function(id) {
        populateTeamData( id );
    };
};

function populateTeamData(id) {
    let myPlayers = [];
    let myPlayersFullDetail = [];
    let listOfRoleFilters = ["FULL"];
    let listOfTeamFilters = ["FULL"];
    let filter = { role: "FULL", team: "FULL" };
    ALL_PLAYERS.map(function(player) {
        if(listOfRoleFilters.indexOf(player.role) == -1) listOfRoleFilters.push(player.role);
        if(listOfTeamFilters.indexOf(player.team) == -1) listOfTeamFilters.push(player.team);
    });

    function displayPlayerList() {
        let list = "";    
        ALL_PLAYERS.map(function(player) {
            if( myPlayers.indexOf(player.Id) == -1 ) {
                if(((filter.role == "FULL") || (filter.role == player.role)) && ((filter.team == "FULL") || (filter.team == player.team))) {
                    list+= "<li>";
                    list+= "<span class='playerName'>"+player.Name+"</span>";
                    list+= "<span class='playerTeam'>"+player.team+"</span>";
                    list+= "<span class='playerPrice'>"+player.price+"</span>";
                    list+= "<span class='addPlayer' data='"+player.Id+"'>+</span></li>";
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
            leftColData += (role == filter.role) ? "<li class='selected'>"+role+"</li>" : "<li>"+role+"</li>";
        });
        listOfTeamFilters.map(function(team){
            rightColData += (team == filter.team) ? "<li class='selected'>"+team+"</li>" : "<li>"+team+"</li>";
        });
        myPlayersFullDetail = [];
        let myBatsmanData = "";
        let myBowlerData = "";
        let myAllRounderData = "";
        let myWicketKeeperData = "";
        ALL_PLAYERS.map(function(player){
            if(myPlayers.indexOf(player.Id) > -1) {
                myPlayersFullDetail.push(player);
                let mPDate = "<li>";
                mPDate += "<div class='iconContainer'><img class='allCountries "+player.team.replace(/ /gi, "")+"'/>";
                mPDate += "<img class='roles' src='/icons/roles/"+player.role+".png'/></div>";
                mPDate += "<div class='playerInfo'><span class='pName'>"+player.Name+"</span>";
                mPDate += "<span class='pPrice'>"+player.price+"</span></div>";
                mPDate += "<img src='/icons/delete.png' class='deletePlayer' data='"+player.Id+"'></li>";

                switch(player.role) {
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
        $("#budgetLeftMessage").html("Budget Left: " + response.budgetLeft);
        $("#subsLeftMessage").html("Substitutions Left: 26");
        $("#saveteam").css("visibility", ( response.isError ? "hidden" : "visible" ));

        $( ".leftTeamItems li" ).unbind( "click" );
        $( ".leftTeamItems li" ).bind("click", function() {
            $( ".leftTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.role = $(this)[0].innerText;
            fillDataInUI();
        });
        $( ".rightTeamItems li" ).unbind( "click" );
        $( ".rightTeamItems li" ).bind("click", function() {
            $( ".rightTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.team = $(this)[0].innerText;
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