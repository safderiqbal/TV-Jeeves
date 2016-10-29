const keys      = require('../keys.json');

const sky       = require('./sky');
const clockwork = require('clockwork')({key : keys.clockwork});
const omdb      = require('omdb');

// OMDB

let getFromTitle = (title, callback) => {
    omdb.get({title}, true, (err, show) => {
        if (err)
            return callback(err);

        if (!show)
            return callback({error : 'Show not found'});

        callback(null, show);
    })
};

exports.getFromTitle = (req, res) => {
    getFromTitle(req.params.title, (error, data) => {
        if (error)
            res.status(400).send(error);

        res.send(data);
    });
};

// Clockwork SMS

let sendSms = (to, content, callback) => {
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