<?php
session_start();

class DatabaseManager {

    private $pdo;
    private static $sharedInstance;


    public static function getSharedInstance() {
        if(isset(self::$sharedInstance)) {
            return self::$sharedInstance;
        }
        self::$sharedInstance = new self(); // eq. DatabaseManager
        return self::$sharedInstance;
    }

    private function __construct() {
        $options = array(
            PDO::MYSQL_ATTR_INIT_COMMAND => 'SET NAMES utf8'
        );
        $this->pdo = new PDO('mysql:host=localhost;
       dbname=gr_prod', 'garbage_r', 'oLq1JZe9rcJZo8if', $options);
    }

    public function getPDO() { return $this->pdo; }

    public function getAll($sql, $params = null, $options = PDO::FETCH_ASSOC) {
        if($params != null) {
            $statement = $this->pdo->prepare($sql);
            $statement->execute($params);
            return $statement->fetchAll($options);
        }
        $statement = $this->pdo->query($sql);

        return $statement->fetchAll($options);
    }

    public function exec($sql, $params = []) {
        $statement = $this->pdo->prepare($sql);
        return (int)$statement->execute($params);
    }

    public function getObject($row){

        $query = $this->getPDO()->prepare('SELECT room_list_players.* FROM room_list_players WHERE room_list_players.room_list="' .  $row .'"');
        //$query->execute();

        $query->execute();
        $result = $query->fetchAll(PDO::FETCH_GROUP);

        return $result;
    }

}

?>
