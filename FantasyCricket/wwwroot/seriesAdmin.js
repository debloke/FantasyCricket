"use strict";
var ALL_PLAYERS = [];
 /****************************************************************************
 * @function       Page Load                                                 *
 * @brief          Triggeres when page is loaded                             *
 * @param          None                                                      *
 * @returns        N/A                                                       *
 ****************************************************************************/
$(function() {
    // DUMMY
    // TO BE REPLACED WITH REAL DATA
    let seriesData = [
        {
            "SeriesName" : "ICC World Cup",
            "Type" : "ODI",
            "Matches" : [
                { "Date" : "30 May, 2019", "Team1" : "England", "Team2" : "South Africa" },
                { "Date" : "31 May, 2019", "Team1" : "West Indies", "Team2" : "Pakistan" },
                { "Date" : "1 Jun, 2019", "Team1" : "New Zealand", "Team2" : "Sri Lanka" },
                { "Date" : "1 Jun, 2019", "Team1" : "Australia", "Team2" : "Afganistan" }
            ]
        },
        {
            "SeriesName" : "ICC Champions Trophy",
            "Type" : "ODI",
            "Matches" : [
                { "Date" : "3 May, 2020", "Team1" : "India", "Team2" : "South Africa" },
                { "Date" : "4 May, 2020", "Team1" : "West Indies", "Team2" : "Pakistan" },
                { "Date" : "1 Jun, 2020", "Team1" : "England", "Team2" : "Sri Lanka" },
                { "Date" : "2 Jun, 2020", "Team1" : "Bangladesh", "Team2" : "Afganistan" }
            ]
        }
    ];
    processData(seriesData);
});

 /****************************************************************************
 * @function       processData                                               *
 * @brief          Process data once we have it from http call               *
 * @param          {Object} data - Data to be rendered                       *
 * @returns        N/A                                                       *
 ****************************************************************************/
function processData(data) {
    let header = "<span id='AddSeries'>+</span>";
    let seriesTitleContent = "<ul>";
    data.map(function(sData) {
        seriesTitleContent += "<li>" + sData.SeriesName + "</li>";
    });
    seriesTitleContent += "</ul>";
    $(".seriesLeftBlock").html(header + seriesTitleContent);
    
    $("#AddSeries").unbind("click");
    $("#AddSeries").bind("click", function() {
        openPopupAndCreateRecord(data);
    });
    $(".seriesLeftBlock ul li").unbind("click");
    $(".seriesLeftBlock ul li").bind("click", function() {
        $( ".seriesLeftBlock ul li" ).removeClass("selected");
        $(this).addClass("selected");
        let selectedSeries = this.innerText;
        data.map(function(mData){
            if(mData.SeriesName == selectedSeries ) {
                displayInRightContainer(mData);
            }
        });
    });
}

function displayInRightContainer(seriesDesc) {
    let header = "<h3>"+ seriesDesc.SeriesName +"</h3>"
    let seriesMatchesContent = "<table><tr><th>Date</th><th>Team 1</th><th>Team 2</th></tr>";
    if( seriesDesc.Matches.length ) {
        seriesDesc.Matches.map(function(matches) {
            seriesMatchesContent += "<tr>";
            seriesMatchesContent += "<td>"+ matches.Date +"</td>";
            seriesMatchesContent += "<td><img class='allCountries "+ matches.Team1.replace(/ /gi, "") +"'/><span>" + matches.Team1 + "</span></td>";
            seriesMatchesContent += "<td><img class='allCountries "+ matches.Team2.replace(/ /gi, "") +"'/><span>" + matches.Team2 + "</span></td>";
            seriesMatchesContent += "</tr>";
        });
    }
    seriesMatchesContent += "</table>";
    $(".seriesDescription").html(header + seriesMatchesContent);
}

function openPopupAndCreateRecord(data) {
    $("#inputPopup").show();
    $(".close").unbind("click");
    $("#addSeries").unbind("click");
    $("#addSeries").bind("click", function() {
        let obj = {};
        obj.SeriesName = $("#serName").val();
        obj.Type = $("#serType").val();
        obj.Matches = JSON.parse($("#matches").val());
        // Clear all data
        $("#serName").val("");
        $("#serType").val("");
        $("#matches").val("");
        data.push(obj);
        $("#inputPopup").hide();
        processData(data);
    });
    $(".close").bind("click", function() {
        $("#inputPopup").hide();
    });
}