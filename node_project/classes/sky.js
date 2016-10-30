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

let getAllChannelListing = (errorMessage, callback) => {
    let channelNos = setup.channels.map((val) => {
        return val.channelid;
    }).join(',');

    request.get(`http://epgservices.sky.com/tvlistings-proxy/TVListingsProxy/tvlistings.json?channels=${encodeURI(channelNos)}&time=${moment().format('YYYYMMDDHH') + '00'}&dur=119&siteId=1&detail=2`,
        (error, response, body) => {
            if (!!error)
                return callback(error);

            let data = JSON.parse(body);

            if (!data.channels || data.channels.length < 1)
                return callback({error: errorMessage});

            return callback(null, data);
        }
    );
}

exports.matchChannelName = (channelName, numResults, callback) => {
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

exports.matchChannelId = (channelId, callback) => {
    let result = setup.channels.find((element) => {
        return element.channelid === channelId;
    });

    if (!result)
        return callback({error: 'Could not find a matching channel'});
    
    let object = {};
    object.channelno = result.channelno;
    object.title = result.title;
    object.channelid = result.channelid;
    object.epggenre = getEpgGenre(result.epggenre);
    object.genre = getGenre(result.genre);
    object.channeltype = getChannelType(result.channeltype);

    return callback(null, object);
};

exports.getCurrentShow = (channelId, callback) => {
    request.get(`http://epgservices.sky.com/tvlistings-proxy/TVListingsProxy/tvlistings.json?channels=${channelId}&time=${moment().format('YYYYMMDDHHmm')}&dur=119&siteId=1&detail=2`,
        (error, response, body) => {
            if(!!error)
                callback(error);

            let data = JSON.parse(body);

            if (!(data.channels instanceof Array)) { 
                let tmp = [];
                tmp.push(data.channels);
                data.channels = tmp; 
            } 
             
            data.channels = data.channels.map((val) => { 
                return val.program[0]; 
            });
 
            callback(null, data.channels); 
        });
};

exports.getMatchingGenre = (genreId, subGenreId, callback) => {
    subGenreId = subGenreId || '';

    getAllChannelListing('Could not find a matching show of that genre',  (error, data) => {
        if (!!error)
            return callback(error);

        let results = data.channels.filter((val) => {
            return val.program.genre === genreId;
        });

        if (subGenreId !== '') {
            results = results.filter((val) => {
                return val.program.subgenre === subGenreId;
            });
        }

        results = results.map((val) => {
            return val.program;
        });

        callback(null, results);
    });
};

exports.getRandomShow = (eventId, callback) => {
    getAllChannelListing('Could not find the current show you are watching', (error, data) => {
        if (!!error)
            return callback(error);

        return callback(null, data);

        data.channels.map((val) => {
            console.log(val.program);
        });
        
        let result = data.channels.filter((val) => {
            return val.program.eventid === eventId;
        });

        result = result.program;

        callback(null, result);
    });
};