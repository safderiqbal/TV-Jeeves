const keys      = require('../keys.json');

const sky       = require('./sky');
const clockwork = require('clockwork')({key : keys.clockwork});
const omdb      = require('omdb');

// OMDB
let getShowFromTitle = (title, callback) => {

    console.log('omdb - getFromTitle - title : ' + title);
    omdb.get({title}, true, (err, show) => {
        if (err)
            return callback(err);

        if (!show)
            return callback({error : 'Show not found'});

        callback(null, show);
    });
};

let getChannelFromName = (channelName, results, callback) => {
    results = results || 5;
    sky.matchChannel(channelName, results, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
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
    })
};

exports.getChannelFromName = (req, res) => {
    getChannelFromName(req.params.title, req.query.results, (error, data) => {
        if (!!error)
            res.status(400).send(error);

        res.send(data);
    })
}