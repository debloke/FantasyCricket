"use strict";
let teamScreenData = new teamScreen();
let liveScoreScreenData = new liveScoreScreen();
let myLeagueScreenData = new myLeagueScreen();
let mainContentId = "#mainContent";

// This holds classes for all child pages
let targetScreens = [
    teamScreenData,
    myLeagueScreenData,
    liveScoreScreenData
];

 /****************************************************************************
 * @function       Page Load                                                 *
 * @brief          Triggeres when page is loaded                             *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
$(function () {
    if (!localStorage.loggedInUser) window.location.href = "index.html";
    
    $(".myMenu li a").click( function() {
        $(".myMenu li a").removeClass("current");
        $(this).addClass("current");
        targetScreens[$(this.parentElement).index()].returnDataForDisplay(mainContentId);
    });

    let display = "<div>" + localStorage.loggedInUserDisplay + "</div>";
    display += "<div><a onclick='logout()'>Logout</a></div>";
    $("#accountDetail").html(display);
    $("#accountInfo").click(function () {
        $("#accountDetail").toggle();
    });
    targetScreens[0].returnDataForDisplay(mainContentId);
});


function logout() {
    let utility = new UtilityClass();
    // AJAX call
    utility.postRequest(
        "/api/user/logout",
        function (data) {
            localStorage.clear();
            window.location.href = "index.html";
        },
        function (err) {
            localStorage.clear();
            window.location.href = "index.html";
        }
    );
}