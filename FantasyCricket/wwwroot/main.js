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


    display += "<div class='flipswitch'>" +
        "<input type='checkbox' name='flipswitch' class='flipswitch-cb' id='fs' checked>" +
        "<label class='flipswitch-label' for='fs'>" +
        "<div class='flipswitch-inner'></div><div class='flipswitch-switch'></div>" +
        "</label></div>";
    display += "<div><a onclick='logout()' href='#'>Logout</a></div>";
    $("#accountDetail").html(display);
    $("#accountInfo").click(function () {
        $("#accountDetail").toggle();
    });
    $("#fs").change(function () {
        if ($(this)[0].checked) {
            $("body").css("background", "url('images/background.png')");
            $("*").css("color", "black");
            $("nav ul li a div").css("background-image", "-webkit-linear-gradient(#32358e 45%, #52592a)");
            $("nav ul li a:hover, nav ul li a.current, .selected").css({ "border":"4px solid #5554a2", "border-left":"none", "border-right":"none", "border-bottom":"none" });
        }
        else {
            $("body").css("background", "url('images/bg.jpg')");
            $("*").css("color", "#d1dce2");
            $("nav ul li a div").css("background-image", "-webkit-linear-gradient(#ffffff 45%, #52592a)");
            $("nav ul li a:hover, nav ul li a.current, .selected").css({ "border": "4px solid #fff", "border-left": "none", "border-right": "none", "border-bottom": "none" });
            $("#accountDetail *").css("color", "#123");
        }
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