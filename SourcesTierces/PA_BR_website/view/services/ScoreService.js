let ScoreService = function() {};

ScoreService.list = function(callback) {
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if(xhr.readyState === 4) {
            //console.log(xhr.responseText);
            let json = JSON.parse(xhr.responseText);
            let score = json.map(function(o) {
                return Score.fromJSON(o);
            });
            callback(score);
        }
    };
    xhr.open('GET', '../../services/account/list.php');
    xhr.send();
};
