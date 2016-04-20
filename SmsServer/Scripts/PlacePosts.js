/// <reference path="jquery-1.10.2.js" />

//TODO Get current position, or else, just place in the middle of denmark

var postlistElement = $('#postlist');
var popup = L.popup();

var allMarkers = [];
var markersAndPosts = {};

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
        updatePostsOnMap();
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
            var newHtml = $('.showPostMarker').clone();
            $('.posttitle', newHtml).html(post.Title);
            m.bindPopup(newHtml.html());
            m.addTo(map);
            allMarkers.push(m);
            markersAndPosts[post.Id] = m;
        }
    }
};

var updatePostsOnMap = function () {
    $.getJSON('/api/PostsApi/' + raceid, {}, function (data) {
        posts = data;
        deleteAllMarkes();
        postlistElement.empty();
        showPosts(posts);
        drawLines();
    });
};

var deleteAllMarkes = function () {
    var markers = allMarkers;
    for (var i = markers.length - 1; i >= 0; i--) {
        map.removeLayer(markers[i]);
    }
    allMarkers = [];
    markersAndPosts = {};
};

var drawLines = function () {
    for (var i = posts.length - 1; i >= 0; i--) {
        var post = posts[i];
        if (markersAndPosts[post.Id]) {
            var startPoint = markersAndPosts[post.Id]._latlng;
            for (var j = post.Answers.length - 1; j >= 0; j--) {
                if (markersAndPosts[post.Answers[j].NextPostId]) {
                    var firstpolyline = new L.Polyline([startPoint,markersAndPosts[post.Answers[j].NextPostId]._latlng] , {
                        color: 'red',
                        weight: 3,
                        opacity: 0.5,
                        smoothFactor: 1
                    });
                    firstpolyline.addTo(map);
                }
            }
        }
    }
};

$(document).ready(function () {
    updatePostsOnMap();
});
