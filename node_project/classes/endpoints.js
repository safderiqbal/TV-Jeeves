const keys      = require('../keys.json');

const sky       = require('./sky');
const clockwork = require('clockwork')({key : keys.clockwork});
const omdb      = require('omdb');

// OMDB
let getShowFromTitle = (title, callback) => {
    console.log('omdb - getShowFromTitle - title : ' + title);
    omdb.get({title}, true, (err, show) => {
        if (err)
            return callback(err);

        if (!show)
            return callback({error : 'Show not found'});

        callback(null, show);
    });
};

exports.getShowFromTitle = (req, res) => {
    getShowFromTitle(req.params.title, (error, data) => {
        if (!!error)
            res.status(400).send(error);

        res.send(data);
    });
};

// Clockwork SMS

let sendSms = (to, content, callback) => {
    console.log('clockwork - sendSms - to : ' + to + ' content : ' + content);
    clockwork.sendSms({to, content}, (err, response) => {
        if (err)
            return callback(err);

        if (!response)
            return callback({error : 'No response'});

        callback(null, response);
    });
};

exports.sendSms = (req, res) => {
    sendSms(req.params.to, req.params.body, (error, data) => {
        if (error)
            res.status(400).send(error);

        res.send(data);
    });
};

// Sky

let getChannelFromName = (channelName, results, callback) => {
    console.log('sky - getChannelFromName - channelName : ' + channelName + ' results : ' + results);    
    results = results || 5;
    sky.matchChannelName(channelName, results, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
    });
};

let getChannelFromId = (channelId, callback) => {
    sky.matchChannelId(channelId, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
    });
}

let getCurrentShowFromChannel = (channelId, callback) => {
    console.log('sky - getCurrentShowFromChannel - channelId : ' + channelId);  
    sky.getCurrentShow(channelId, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
    });
};

let getMatchingGenre = (genreId, subGenreId, callback) => {
    console.log('sky - getMatchingGenre - genreId : ' + genreId + ' subGenreId : ' + subGenreId);  
    sky.getMatchingGenre(genreId, subGenreId, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
    });
};

exports.getChannelFromName = (req, res) => {
    getChannelFromName(req.params.channelName, req.query.results, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};

exports.getChannelFromId = (req, res) => {
    getChannelFromId(req.params.channelId, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};

exports.getCurrentShowFromChannel = (req, res) => {
    getCurrentShowFromChannel(req.params.channelId, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};

exports.getMatchingGenre = (req, res) => {
    getMatchingGenre(req.params.genreId, req.query.subGenreId, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};