<?php

ini_set('display_errors', 1);

require_once __DIR__ ."/../../dao/AccountDAO.php";
header('Content-Type: application/json');

$scoreList = AccountDAO::listPlayerScores();

echo json_encode($scoreList);