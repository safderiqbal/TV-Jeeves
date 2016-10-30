const express = require('express');
const http = require('http');
const errorHandler = require('morgan');
const methodOverride = require('method-override');
const bodyParser = require('body-parser');

const endpoints = require('./classes/endpoints');

let app = express();
let server = http.createServer(app);

app.set('port', process.env.PORT || 3000);
app.use(methodOverride());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({
    extended: true
}));


app.get('/omdb/:title', endpoints.getShowFromTitle);

app.get('/sms/:to/:body', endpoints.sendSms);

app.get('/sky/channel/id/:channelId', endpoints.getChannelFromId);
app.get('/sky/channel/name/:channelName', endpoints.getChannelFromName);
app.get('/sky/channel/current/:channelId', endpoints.getCurrentShowFromChannel);
app.get('/sky/genre/:genreId', endpoints.getMatchingGenre);
//app.get('/sky/channel/upcoming/:channel', endpoints.getChannelUpcoming);

// log 404s
app.get('/*', (req, res) => {
    console.log('Error: No Request Found - ' + req.url);
    res.send('Error: No Request Found');
});

// development error handler
if (process.env.NODE_ENV === 'development') {
    app.use(errorHandler());
}

// no stacktraces leaked to user
app.use((err, req, res) => {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

server.listen(app.get('port'), () =>{
    console.log('Express server listening on port ' + app.get('port'));
});