<?php

ini_set('display_errors', 1);

require_once __DIR__ ."/../../dao/RoomDAO.php";
header('Content-Type: application/json');

$roomList = RoomDAO::listRoom();

echo json_encode($roomList);