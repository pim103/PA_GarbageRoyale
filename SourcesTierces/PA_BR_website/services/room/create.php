<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 04/07/2019
 * Time: 16:50
 */

ini_set('display_errors', 1);

require_once __DIR__ ."/../../dao/RoomDAO.php";
header('Content-Type: application/json');

print_r($_POST);

$roomCreated = RoomDAO::createRoom($_POST['name'], $_POST['maxPlayer'], $_POST['creatorUserId']);

if($roomCreated){
    http_response_code(201);
}
else{
    http_response_code(406);
}