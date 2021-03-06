﻿/// <reference path="jquery-1.10.2.js" />

//TODO Get current position, or else, just place in the middle of denmark

var postlistElement = $('#postlist');
var popup = L.popup();

var allMarkers = [];
var markersAndPosts = {};

var allLines = [];
var allArrows = [];

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
        elem.innerText = (i + 1) + ': ' + post.Title;
        $(elem).attr("data-postid", post.Id);
        $(elem).mouseover(function (e) {
            var tmpMar = markersAndPosts[$(this).data().postid];
            tmpMar.openPopup();
        });
        $(elem).mouseout(function (e) {
            var tmpMar = markersAndPosts[$(this).data().postid];
            tmpMar.closePopup();
        });
        postlistElement.append(elem);
        if (post.lattitude != 0 || post.longitude != 0) {
            var m = L.marker([post.lattitude, post.longitude]);
            var newHtml = $('.showPostMarker').clone();
            $('.posttitle', newHtml).html((i + 1) + ': ' + post.Title);
            console.log($('.edit', newHtml));
            $('a', newHtml).attr('href', '/posts/edit/' + post.Id);
            m.bindPopup(newHtml.html());
            m.addTo(map);
            allMarkers.push(m);
            markersAndPosts[post.Id] = m;
        }
    }
    if (allMarkers.length > 0) {
        var group = new L.featureGroup(allMarkers);
        map.fitBounds(group.getBounds());
    }
};

//From: http://stackoverflow.com/questions/4907843/open-a-url-in-a-new-tab-and-not-a-new-window-using-javascript
function OpenInNewTab(url) {
    var win = window.open(url, '_blank');
    win.focus();
}

var updatePostsOnMap = function () {
    $.getJSON('/api/getAllPosts/' + raceid, {}, function (data) {
        posts = data;
        deleteAllMarkes();
        deleteAllLinesAndArrows();
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

var deleteAllLinesAndArrows = function () {
    for (var i = allLines.length - 1 ; i >= 0; i--) {
        map.removeLayer(allLines[i]);
        map.removeLayer(allArrows[i]);
    }
    allLines = [];
    allArrows = [];
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
                    allLines.push(firstpolyline);
                    var ah = L.polylineDecorator(firstpolyline).addTo(map);
                    ah.setPatterns([
                        { offset: '50%', repeat: 0, symbol: L.Symbol.arrowHead({ pixelSize: 15, polygon: false, pathOptions: { stroke: true } }) }
                    ]);
                    allArrows.push(ah);
                }
            }
        }
    }
};

$(document).ready(function () {
    updatePostsOnMap();
});
