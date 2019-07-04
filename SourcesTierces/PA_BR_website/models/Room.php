<?php
/**
 * Created by PhpStorm.
 * User: Medhi
 * Date: 03/07/2019
 * Time: 18:41
 */

class Room implements JsonSerializable
{
    private $id;
    private $name;
    private $currentPlayer;
    private $maxPlayer;
    private $creatorId;
    private $listPlayerRoom;
    private $statement;

    /**
     * Room constructor.
     * @param $id
     * @param $name
     * @param $currentPlayer
     * @param $maxPlayer
     * @param $creatorId
     * @param $listPlayerRoom
     * @param $statement
     */
    public function __construct($id, $name, $currentPlayer, $maxPlayer, $creatorId, $listPlayerRoom, $statement)
    {
        $this->id = $id;
        $this->name = $name;
        $this->currentPlayer = $currentPlayer;
        $this->maxPlayer = $maxPlayer;
        $this->creatorId = $creatorId;
        $this->listPlayerRoom = $listPlayerRoom;
        $this->statement = $statement;
    }

    /**
     * @return mixed
     */
    public function getId()
    {
        return $this->id;
    }

    /**
     * @param mixed $id
     */
    public function setId($id)
    {
        $this->id = $id;
    }

    /**
     * @return mixed
     */
    public function getName()
    {
        return $this->name;
    }

    /**
     * @param mixed $name
     */
    public function setName($name)
    {
        $this->name = $name;
    }

    /**
     * @return mixed
     */
    public function getCurrentPlayer()
    {
        return $this->currentPlayer;
    }

    /**
     * @param mixed $currentPlayer
     */
    public function setCurrentPlayer($currentPlayer)
    {
        $this->currentPlayer = $currentPlayer;
    }

    /**
     * @return mixed
     */
    public function getMaxPlayer()
    {
        return $this->maxPlayer;
    }

    /**
     * @param mixed $maxPlayer
     */
    public function setMaxPlayer($maxPlayer)
    {
        $this->maxPlayer = $maxPlayer;
    }

    /**
     * @return mixed
     */
    public function getCreatorId()
    {
        return $this->creatorId;
    }

    /**
     * @param mixed $creatorId
     */
    public function setCreatorId($creatorId)
    {
        $this->creatorId = $creatorId;
    }

    /**
     * @return mixed
     */
    public function getListPlayerRoom()
    {
        return $this->listPlayerRoom;
    }

    /**
     * @param mixed $listPlayerRoom
     */
    public function setListPlayerRoom($listPlayerRoom)
    {
        $this->listPlayerRoom = $listPlayerRoom;
    }

    /**
     * @return mixed
     */
    public function getStatement()
    {
        return $this->statement;
    }

    /**
     * @param mixed $statement
     */
    public function setStatement($statement)
    {
        $this->statement = $statement;
    }

    public function jsonSerialize()
    {
        return get_object_vars($this);
    }
}
