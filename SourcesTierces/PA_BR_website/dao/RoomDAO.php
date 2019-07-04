<?php

require_once __DIR__ ."/../models/Room.php";
require_once __DIR__ ."/../utils/functions.php";
require_once __DIR__ ."/../utils/DatabaseManager.php";

ini_set('display_errors', 1);


class RoomDAO
{
    private static function roomFromRows(&$rows)
    {
        $arr = [];
        foreach ($rows as &$row) {
            $arr[] = RoomDAO::roomFromRow($row);
        }
        return $arr;
    }

    private static function roomFromRow(&$row)
    {
        $room_players = DatabaseManager::getSharedInstance()->getObject($row['list_players_room']);

        $room = new Room(
            $row['id'],
            $row['name'],
            $row['current_players'],
            $row['max_players'],
            $row['creator_user_id'],
            $room_players,
            $row['statement']
        );

        return $room;
    }

    public static function createRoom($name, $maxPlayer, $creatorUserId){
        $db = DatabaseManager::getSharedInstance();
        $query = 0;

        $listId = self::getListId();

        $queryAddPlayer = $db->exec('INSERT INTO room_list_players (room_list, player_id, isConnected) VALUES (?, ?, ?)', [$listId, $creatorUserId, 1]);

        if ($queryAddPlayer){
            $query = $db->exec('INSERT INTO room_list (name, current_players, max_players, creator_user_id, list_players_room, statement) VALUES (?, ?, ?, ?, ?, ?)', [$name, 1, (int)$maxPlayer, $creatorUserId, $listId, 0]);
        }

        return $query != 0;
    }

    public static function listRoom(){
        $rows = DatabaseManager::getSharedInstance()->getAll('SELECT room_list.* FROM room_list WHERE statement>=0');
        return RoomDAO::roomFromRows($rows);
    }

    private static function getListId(){
        $list_id = DatabaseManager::getSharedInstance()->getPDO()->prepare("SELECT room_list FROM room_list_players ORDER BY room_list DESC");
        $list_id->execute();
        $id = $list_id->fetch(PDO::FETCH_ASSOC);
        //print_r($id);

        $id['room_list']++;
        return (int)$id['room_list'];
    }

}