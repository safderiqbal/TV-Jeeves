const keys      = require('../keys.json');

const sky       = require('./sky');
const clockwork = require('clockwork')({key : keys.clockwork});
const omdb      = require('omdb');
const tvdb      = require('./tvdb');

//TVDB

let getFromTvdb = (title, callback) => {
    tvdb.getFromTvdb(title, (error, data) => {
        if (!!error)
            return callback(error);

        return callback(null, data);
    });
};

exports.getFromTvdb = (req, res) => {
    getFromTvdb(req.params.title, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};

// OMDB
let getShowFromTitle = (title, callback) => {
    console.log('omdb - getShowFromTitle - title : ' + title);
    omdb.get({title}, true, (err, show) => {
        if (!!err)
            return callback(err);

        if (!show)
            return callback({error : 'Show not found'});

        callback(null, show);
    });
};

let getShowPosterFromTitle = (title, callback) => {
    console.log('omdb - getShowPosterFromTitle - title : ' + title);

    omdb.get({title}, true, (err, show) => {
        if (!!err)
            return callback(err);

        if (!show)
            return callback({error : 'Show not found'});

        callback(null, show);
    });
};

exports.getShowFromTitle = (req, res) => {
    getShowFromTitle(req.params.title, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
};

exports.getShowPosterFromTitle = (req, res) => {
    getShowPosterFromTitle(req.params.title, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
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

let getFullChannelRef = (shows, callback) => {
    shows.forEach((show) => {
        sky.matchChannelId(show.channelid, (error, channelData) => {
            if (!!error)
                return callback(error);

            delete show.channelid;
            show.channel = channelData;
        });
    });

    return callback(null, shows);
};

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
    console.log('sky - getChannelFromId - channelId : ' + channelId);  
    sky.matchChannelId(channelId, (error, data) => {
        return !error
            ? callback(null, data)
            : callback(error);
    });
}

let getCurrentShowFromChannel = (channelId, callback) => {
    console.log('sky - getCurrentShowFromChannel - channelId : ' + channelId);  
    sky.getCurrentShow(channelId, (error, showData) => {
        if (!!error)
            return callback(error);

        getFullChannelRef(showData, (error, data) => {
            if (!!error)
                return callback(error);

            callback(null, data);
        });
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

let getGenreWithChannel = (genreId, subGenreId, callback) => {
    console.log('sky - getGenreWithChannel - genreId : ' + genreId + ' subGenreId : ' + subGenreId);

    sky.getMatchingGenre(genreId, subGenreId, (error, showData) => {
        if (!!error)
            return callback(error);

        getFullChannelRef(showData, (error, data) => {
            if (!!error)
                return callback(error);

            callback(null, data);
        });
    });
};

let getRandomShow = (callback) => {
    console.log('sky - getRandomShow');

    sky.getRandomShow((error, showData) => {
        if (!!error)
            return callback(error);

        if (!showData)
            return callback({error: 'No show data returned'});

        sky.matchChannelId(showData.channelid, (error2, channelData) => {
            if (!!error2)
                return callback(error2);

            showData.channel = channelData;
            delete showData.channelid;

            return callback(null, showData);
        });
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

exports.getGenreWithChannel = (req, res) => {
    getGenreWithChannel(req.params.genreId, req.query.subGenreId, (error, data) => {
        if (!!error)
            return res.status(400).send(error);

        return res.send(data);
    });
}

exports.getRandomShow = (req, res) => {
    getRandomShow((error, data) => {
        if (!!error)
            return res.status(400).send(error);
            
        return res.send(data);
    });
};