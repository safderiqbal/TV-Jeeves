const request = require('request');
const xml2js = require('xml2js');

let getSeriesId = (searchName, callback) => {
    request.get(`http://www.thetvdb.com/api/GetSeries.php?seriesname=${encodeURI(searchName)}&language=en`,
        (error, response, body) => {
            if (!!error)
                return callback(error);

            xml2js.parseString(body, {trim: true}, (err, result) => {
                if (!!err)
                    return callback(err);

                if (!result)
                    return callback({error: 'No result found'});

                return callback(null, result.Data.Series[0].seriesid[0]);
            });
        });
};

let getBannerDetails = (seriesId, callback) => {
    request.get(`http://thetvdb.com/api/4144331619000000/series/${seriesId}/banners.xml`,
        (error, response, body) => {
            if (!!error)
                return callback(error);

            xml2js.parseString(body, {trim:true}, (err, result) => {
                if (!!err)
                    return callback(err);

                if (!result)
                    return callback({error: 'No result found for series name'});

                return callback(null, result);
            });
        });
};

let getPosterUrl = (bannersData) => {
    let banners = bannersData.Banners.Banner;
    
    for(let i = 0; i < banners.length; i++) {
        let banner = banners[i].BannerPath[0];
        if (banner.substring(0, 7) === 'posters')
            return {image: `http://thetvdb.com/banners/${banner}`};
    }

    return null;
};

exports.getFromTvdb = (searchName, callback) => {
    getSeriesId(searchName, (error, data) => {
        if (!!error)
            return callback(error);

        if (!data)
            return callback({error: 'No result found for series ID'});

        getBannerDetails(data, (error, posterData) => {
            if (!!error)
                return callback(error);

            let poster = getPosterUrl(posterData);

            if (!poster)
                return callback({error: 'No result found for poster URL'});

            return callback(null, poster);
        });
    });
};