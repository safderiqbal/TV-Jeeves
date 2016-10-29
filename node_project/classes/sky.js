const setup = require('../resources/init');
const fuzzy = require('fuzzy');
const request = require('request');
const moment = require('moment');

let getChannelType = (id) => {
    return setup.types.filter((element) => {
        if (id === element.typeid)
            return element;
    });
};

let getEpgGenre = (id) => {
    return setup.epggenre.filter((element) => {
        if (id === element.genreid)
            return element;
    });
};

let getGenre = (id) => {
    return setup.genre.filter((element) => {
        if (id === element.genreid)
            return element;
    });
};

exports.matchChannel = (channelName, numResults, callback) => {
    const options = {
        extract: (ele) => {
            return ele.title;
        }
    };
    const results = fuzzy.filter(channelName, setup.channels, options);
    
    if (results.length > 0) {
        let result = results.slice(0, numResults);

        return callback(null, result.map((match) => {
            let object = {};
            let val = match.original;

            object.channelno = val.channelno;
            object.title = val.title;
            object.channelid = val.channelid;
            object.epggenre = getEpgGenre(val.epggenre);
            object.genre = getGenre(val.genre);
            object.channeltype = getChannelType(val.channeltype);

            return object;
        }));
    }

    return callback({error: 'Could not find a matching channel'});
};

exports.getCurrentShow = (channelId, callback) => {
    request.get(`http://epgservices.sky.com/tvlistings-proxy/TVListingsProxy/tvlistings.json?channels=${channelId}&time=${moment().format('YYYYMMDDHH') + '00'}&dur=119&siteId=1&detail=2`,
        (error, response, body) => {
            if(!!error)
                callback(error);

            let data = JSON.parse(body);

            callback(null, data.channels.program[0]);
        });
};