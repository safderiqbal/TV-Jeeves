const sky = require('./sky');
const clockwork = require('./clockwork');
const omdb = require('omdb');

let getShowFromTitle = (title, callback) => {
    omdb.get({title}, true, (err, show) => {
        if (err)
            return callback(err);

        if (!show)
            return callback({error: 'Show not found'});

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

exports.getChannelFromName = (req, res) => {
    getChannelFromName(req.params.title, req.query.results, (error, data) => {
        if (!!error)
            res.status(400).send(error);

        res.send(data);
    })
}