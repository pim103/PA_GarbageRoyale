function fetchScores() {
    let mainDiv = document.getElementById("scoreboard-div");
    ScoreService.list(function (scores) {
        let div = document.createElement('div');
        let res = scores.map(function(t) {
            return t.toHTML();
        });
        //console.log(res);
        //div.appendChild(res);
        div.id = "score-div";
        res.forEach(div.appendChild.bind(div));
        mainDiv.appendChild(div);
    });
}

fetchScores();
