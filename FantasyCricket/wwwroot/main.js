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
$(function() {
    if(!sessionStorage.loggedInUser) window.location.href = "index.html";
    
    $(".myMenu li a").click( function() {
        $(".myMenu li a").removeClass("current");
        $(this).addClass("current");
        targetScreens[$(this.parentElement).index()].returnDataForDisplay(mainContentId);
    });
    targetScreens[0].returnDataForDisplay(mainContentId);
});