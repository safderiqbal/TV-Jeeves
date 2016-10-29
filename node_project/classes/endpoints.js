const sky = require('./sky');
const clockwork = require('./clockwork');
const omdb = require('omdb');

let getFromTitle = (title, callback) => {
    omdb.get({title}, true, (err, show) => {
        if (err)
            return callback(err);

        if (!show)
            return callback({error: 'Show not found'});

        callback({}, show);
    })
};

exports.getFromTitle = (req, res) => {
    getFromTitle(req.params.title, (error, data) => {
        if (error)
            res.status(400).send(error);

        res.send(data);
    });
};