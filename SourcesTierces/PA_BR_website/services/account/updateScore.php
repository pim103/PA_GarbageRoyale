<?php

ini_set('display_errors', 1);

require_once __DIR__ ."/../../dao/AccountDAO.php";
header('Content-Type: application/json');

$finalScore = AccountDAO::updatePlayerScore($_POST['userid'], $_POST['addScore']);

if($finalScore){
    http_response_code(200);
}
else{
    http_response_code(406);
}