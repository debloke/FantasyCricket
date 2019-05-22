let UtilityClass = function() {
    let that = this;
    that.baseURL = "http://cricketfantasy.azurewebsites.net";
    
    /**************************************************
     * @function     getRequest
     * @brief        Make HTTP GET calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.getRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            {url: (that.baseURL + url)}, 
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
     * @return       N/A
     **************************************************/
    that.postRequest = function(url, successCB, errorCB, data) {
        let obj = {url: (that.baseURL + url), type: "POST"};
        if(data) {
            obj.data = JSON.stringify(data);
            obj.contentType = "application/json";
        }
        makeHTTPCall(
            obj, 
            successCB,
            errorCB);
    };
    
    /**************************************************
     * @function     putRequest
     * @brief        Make HTTP PUT calls
     * @param        {String} url - URL for HTTP request
     * @param        {Callback} successCB - Callback for success
     * @param        {Callback} errorCB - Callback for error
     * @return       N/A
     **************************************************/
    that.putRequest = function(url, successCB, errorCB) {
        makeHTTPCall(
            {url: (that.baseURL + url), type: "PUT"}, 
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
            {url: (that.baseURL + url), type: "DELETE"}, 
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
        // AJAX call
        $.ajax(obj)
        .done(function( data ) {
            successCB(data);
        }).fail(function(err) {
            errorCB(err);
        });
    };  
};