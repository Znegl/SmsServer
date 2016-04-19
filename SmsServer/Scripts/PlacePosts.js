/// <reference path="jquery-1.10.2.js" />

//TODO Get current position, or else, just place in the middle of denmark

var postlistElement = $('#postlist');
var popup = L.popup();

var map = L.map('mapid').setView([56.0107, 10.9204], 7);
L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

var onMapClick = function (e) {
    latestPos = e.latlng;
    popup
        .setLatLng(e.latlng)
        .setContent($('.placePostInput').html())
        .openOn(map);
};
map.on('click', onMapClick);

var sendPlacementDataToServer = function () {
    var postid = $('#postToPlace').val();
    var data = {
        longitude: parseFloat(latestPos.lng)
        ,lattitude: parseFloat(latestPos.lat)
        //,postid: postid
    };
    //Create html and set it as the popup thingie
    $.ajax({
        beforeSend: function () {
            popup.setContent("Sending data to server...");
        },
        url: "/api/updateLocationForPost/" + postid,//+"/"+data.longitude+"/"+data.lattitude,
        type: "POST",
        data: data
    }).done(function (resData) {
        //console.log(JSON.stringify(resData));
        popup.setContent("Tak for dit input.");
    }).fail(function () {
        popup.setContent("Fejl ved uploading");
    });
};
var posts = [];

var showPosts = function (posts) {
    for (var i = posts.length - 1; i >= 0; i--) {
        var post = posts[i];
        var elem = document.createElement('li');
        elem.innerText = post.Title;
        postlistElement.append(elem);
        if (post.lattitude != 0 || post.longitude != 0) {
            var m = L.marker([post.lattitude, post.longitude]);
            m.addTo(map);
        }
    }
};

$(document).ready(function () {
    $.getJSON('/api/PostsApi/' + raceid, {}, function (data) {
        posts = data;
        showPosts(posts);
    });
});