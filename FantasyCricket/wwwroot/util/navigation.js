function navigateTo(site) {
    let siteObject = {
        "facebook": "https://www.facebook.com/",
        "twitter": "https://twitter.com/",
        "snapchat": "https://www.snapchat.com/"
    };
    if (siteObject[site]) window.location.href = siteObject[site];
}