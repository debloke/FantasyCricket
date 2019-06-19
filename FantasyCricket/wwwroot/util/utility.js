let UtilityClass = function () {
    let that = this;
    that.baseURL = "";
    /* that.baseURL = "https://cricketfantasy.azurewebsites.net/"; */

    /**************************************************
     * @function     getRequest
     * @brief        Make HTTP GET calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.getRequest = function (url, successCB, errorCB) {
        makeHTTPCall(
            addGUIDForRequest({ url: (that.baseURL + url) }), 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     postRequest
     * @brief        Make HTTP POST calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @param        {Object} data - Data to post
     * @param        {String} contentType
     * @param        {Object} headers
     * @return       N/A
     **************************************************/
    that.postRequest = function(url, successCB, errorCB, data, contentType, headers) {
        let obj = {url: (that.baseURL + url), type: "POST"};
        if(data) {
            obj.data = JSON.stringify(data);
            obj.contentType = contentType || "application/json";
        }
        if (headers) obj.headers = headers;
        makeHTTPCall(
            addGUIDForRequest(obj), 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     putRequest
     * @brief        Make HTTP PUT calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @param        {Object} data - Data to put
     * @return       N/A
     **************************************************/
    that.putRequest = function (url, successCB, errorCB, data) {
        let obj = { url: (that.baseURL + url), type: "PUT" };
        if (data) {
            obj.data = JSON.stringify(data);
            obj.contentType = "application/json";
        }
        makeHTTPCall(
            addGUIDForRequest(obj), 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     deleteRequest
     * @brief        Make HTTP DELETE calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.deleteRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            addGUIDForRequest({url: (that.baseURL + url), type: "DELETE"}), 
            successCB,
            errorCB);
    };
    
    /////////////////////////
    //  PRIVATE FUNCTIONS  //
    /////////////////////////
    /**************************************************
     * @function     makeHTTPCall
     * @brief        Make HTTP calls
     * @param        {Object} obj - Object for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    function makeHTTPCall(obj, successCB, errorCB) {
        let UNAUTHORIZED = 401;
        // AJAX call
        $.ajax(obj)
        .done(function( data ) {
            successCB(data);
        }).fail(function (err) {
            if (err.status == UNAUTHORIZED) {
                localStorage.clear();
                window.location.href = "index.html";
            }
            else {
                errorCB(err);
            }
        });
    };

    function addGUIDForRequest(obj) {
        if (localStorage.guid) {
            obj.headers = {};
            obj.headers.Magic = localStorage.guid;
        }
        return obj;
    }
};