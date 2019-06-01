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
            ALL_PLAYERS = values[0].sort(function(a,b) {
                let val1 = a.TeamName;
                let val2 = b.TeamName;
                if (val1 == val2) return 0;
                if (val1 > val2) return 1;
                if (val1 < val2) return -1;
            });
            populateTeamData(id, values[1]);
        });
    };
};

function populateTeamData(id, listOfMyPlayers) {
    let myPlayers = listOfMyPlayers.PlayerIds || [];
    let myPlayersFullDetail = [];
    let listOfRoleFilters = ["BAT", "WK", "ALL", "BOWL", "FULL"];
    let listOfTeamFilters = ["FULL"];
    let filter = { Role: "FULL", TeamName: "FULL" };
    ALL_PLAYERS.map(function(player) {
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
            let isSelectedClass = (role == filter.Role) ? " class='selected'" : "";
            if( role == "FULL" ) {
                leftColData += "<li" + isSelectedClass + " data='"+role+"'>"+role+"</li>";
            }
            else {
                leftColData += "<li" + isSelectedClass + " data='"+role+"'><img src='/icons/roles/"+role+".png'/></li>";
            }
        });
        listOfTeamFilters.map(function(team){
            let isSelectedClass = (team == filter.TeamName) ? " class='selected'" : "";
            if( team == "FULL" ) {
                rightColData += "<li" + isSelectedClass + " data='"+team+"'>ALL</li>";
            }
            else {
                rightColData += "<li" + isSelectedClass + " data='"+team+"'><img class='allCountries "+team.replace(/ /gi, "")+"'/></li>";
            }
        });
        myPlayersFullDetail = [];
        let myBatsmanData = "";
        let myBowlerData = "";
        let myAllRounderData = "";
        let myWicketKeeperData = "";
        let captains = "";
        ALL_PLAYERS.map(function(player){
            if(myPlayers.indexOf(player.pid) > -1) {
                let role = "";
                if(listOfMyPlayers.BattingCaptain == player.pid) {
                    role += "<span class='captain'><img src='/icons/roles/BAT.png'></span>"
                }
                if(listOfMyPlayers.BowlingCaptain == player.pid) {
                    role += "<span class='captain'><img src='/icons/roles/BOWL.png'></span>"
                }
                if(listOfMyPlayers.FieldingCaptain == player.pid) {
                    role += "<span class='captain'><img src='/icons/roles/WK.png'></span>"
                }

                myPlayersFullDetail.push(player);
                let mPData = "<li>";
                player.name = player.name;
                mPData += "<div class='iconContainer'><img class='allCountries "+player.TeamName.replace(/ /gi, "")+"'/>";
                mPData += "<img class='roles' src='/icons/roles/"+player.Role+".png'/></div>";
                mPData += "<div class='playerInfo'><span class='pName'>"+player.name+"</span>";
                mPData += "<span class='pPrice'>"+player.Cost+"</span></div>";
                mPData += role;
                mPData += "<img src='/icons/delete.png' class='deletePlayer' data='"+player.pid+"'></li>";

                switch(player.Role) {
                    case "BAT": myBatsmanData+= mPData; break;
                    case "WK": myWicketKeeperData+= mPData; break;
                    case "ALL": myAllRounderData+= mPData; break;
                    case "BOWL": myBowlerData+= mPData; break;
                    default: break;
                }
            }
        });
        let uiData = "<div class='myTeam'>";

        uiData += "<div id='budgetAndSubsLeftMessage'></div>";
        uiData += "<div><ul>"+myBatsmanData+"</ul></div>";
        uiData += "<div><ul>"+myWicketKeeperData+"</ul></div>";
        uiData += "<div><ul>"+myAllRounderData+"</ul></div>";
        uiData += "<div><ul>"+myBowlerData+"</ul></div>";
        uiData += "<div class='infoteams'><div id='errorMessage'></div>";
        uiData += "<div id='saveteam'>Submit</div></div></div>";

        let innerData = "<div class='leftTeamItems'><ul>"+leftColData+"</ul></div>";
        innerData += "<div class='centerTeamItems'><ul>"+centerColData+"</ul></div>";
        innerData += "<div class='rightTeamItems'><ul>"+rightColData+"</ul></div>";
        uiData += "<div class='teamList'>"+innerData+"</div>";
        $(id).html(uiData);

        let teamValidation = new validator(myPlayersFullDetail);
        let response = teamValidation.runValidations();
        (response.isError) && $("#errorMessage").html(response.error);
        let budgetAndSubs = "";
        budgetAndSubs += "<div class='budgetSub'>Budget Left: " + response.budgetLeft + "</div>";
        budgetAndSubs += "<div class='budgetSub'>Substitutions Left: " + (myPlayers.length ? listOfMyPlayers.RemSubs : "&#8734;</div>");

        $("#budgetAndSubsLeftMessage").html(budgetAndSubs);
        $("#saveteam").css("visibility", ( response.isError ? "hidden" : "visible" ));
        $("#errorMessage").css("visibility", ( response.isError ? "visible" : "hidden" ));

        // Save Team
        $("#saveteam").bind("click", function() {
            openPopupForCaptains(listOfMyPlayers, myPlayersFullDetail);
        });

        $( ".leftTeamItems li" ).unbind( "click" );
        $( ".leftTeamItems li" ).bind("click", function() {
            $( ".leftTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.Role = this.getAttribute("data");
            fillDataInUI();
        });
        $( ".rightTeamItems li" ).unbind( "click" );
        $( ".rightTeamItems li" ).bind("click", function() {
            $( ".rightTeamItems li" ).removeClass("selected");
            $(this).addClass("selected");
            filter.TeamName = this.getAttribute("data");
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

    $(".inputContainer").html(selectBattingCaptain + selectBowlingCaptain + selectFieldingCaptain + submitBtn).css({"background":"#7e7cc3", "width": "auto", "height": "auto", "padding": "20px", "margin": "0px auto", "border-radius": "10px", "left": "40%", "color": "black"});

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