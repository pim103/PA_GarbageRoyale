-- phpMyAdmin SQL Dump
-- version 4.6.6deb4
-- https://www.phpmyadmin.net/
--
-- Client :  localhost:3306
-- Généré le :  Jeu 04 Juillet 2019 à 16:48
-- Version du serveur :  10.1.26-MariaDB-0+deb9u1
-- Version de PHP :  7.0.30-0+deb9u1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données :  `gr_prod`
--

-- --------------------------------------------------------

--
-- Structure de la table `account`
--

CREATE TABLE `account` (
  `id` int(11) NOT NULL,
  `name` varchar(14) NOT NULL,
  `email` varchar(50) NOT NULL,
  `password` char(60) NOT NULL,
  `token` char(32) DEFAULT NULL,
  `userid` char(36) NOT NULL,
  `role` int(11) NOT NULL DEFAULT '0',
  `isActive` tinyint(1) NOT NULL DEFAULT '1',
  `score` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Contenu de la table `account`
--

INSERT INTO `account` (`id`, `name`, `email`, `password`, `token`, `userid`, `role`, `isActive`, `score`) VALUES
(1, 'Kazetheroth', 'medhi.foulgoc@gmail.com', '$2y$10$BgnIEKDdFQlQn3kfjdDbIegzfahSJasM/N5SJYOwVCH/0.iewwesG', NULL, 'avykapcw-a60i-q0pg-5z4g-qspg0l7ydtun', 3, 0, 105),
(4, 'Medhi2703', 'med__27@hotmail.fr', '$2y$10$Qr.OtB5pIKoR45WWLXlN5esIQYY2kTAbEy/l.dH2F9BVLjQte46Ue', NULL, 'bcunp3uc-v3wh-laxs-s22v-5znntqv4qro1', 3, 0, 0),
(5, 'medhif', 'medhi@heolia.eu', '$2y$10$oBuemI2/AnquO11XXzNOe.s4ruZtrsC5ofqMG/2YikOKG0uZpmbM.', NULL, '65qvw5d8-zc4w-hbbi-t8s0-xzzhsadh3bsa', 0, 0, 0),
(6, 'pim', 'pim103@hotmail.fr', '$2y$10$n0xeo53PgA6TTc4AZ8ez0e2iDOqPAWiSIydCbM1v62cGrbFlWZxw.', NULL, '0ga1hh5c-rfvp-zkus-98g1-q2hrydtfcubc', 0, 0, 0),
(7, 'lastuck', 'test@test.fr', '$2y$10$P8HLVSJB.31p9rSTBxvc7OVIcpwB7CzhJP6akUVhxROF/ED/p1ymu', NULL, '8w8v0hka-1c3q-ezw9-msfk-vj6c1cc7cpil', 0, 0, 0),
(8, 'lastuck2', 'test2@test.fr', '$2y$10$CHKzI/FyaPONFcajWfla6OSIOU2OHCl9sPWdVJDhCbL/PF1WEX/PW', NULL, '62vi7888-pr48-44bq-ol66-ousiun8p4mca', 0, 0, 0),
(9, 'pim2', 'pim@hotmail.fr', '$2y$10$jm9nOihNGPBh.WaiI98nLeKhY/AWPfqY7isRQw1ydUCyrD96QQD2q', NULL, 'mrgm80wa-dz0s-zx2l-ph6l-1dy2pba21tnn', 0, 0, 0),
(10, 'test22', 'test2@test2.fr', '$2y$10$xEbcXuKXg9xG1sJ66i3AYObpNa49uXLSQK8tXaiRT0ziGPoj8enkm', NULL, 'mx1dvh7t-q4om-2bft-jipr-ennoy2q7defz', 0, 0, 0),
(11, 'pimpim', 'pimpim@hotmail.fr', '$2y$10$lLzMIXK4TWYTw7zPmbwbfuUtTnZtKWoag7ZFAx9YtrG5o4wonAp4u', NULL, '6d8ixebj-w8ds-6o7g-qyo9-mx7zx0e0k1yq', 0, 1, 0),
(12, 'marcher', 'marcher.pro@gmail.com', '$2y$10$fvksQVAk3WT8p3fsdKbBoeNmIuCtqGht84iN9fv7F.vWiVvGZVWaC', NULL, '8zhzif1e-ydmz-snz3-j8ia-9c7rxt81m1nv', 0, 1, 0),
(13, 'cread', 'mickaelberthelot@outlook.fr', '$2y$10$t3Hm9CccSCJhoBnL3eM1PeHxivevpYif3tin6wt1csj/iZ/k1Yhqy', NULL, 'givf2tsm-i6ko-yq7n-kui5-tj4fsp9rlxu2', 0, 1, 0),
(14, 'Nicky Larcin', 'ruppel.remi@gmail.com', '$2y$10$lQdx5Ut/LyoeyvEQLFUrcumnI1bHEch73Li.d9InfmjUUJidiRrdW', NULL, 'cho3c5wa-7hly-b1s5-ej2u-qo4yssd6ijhu', 0, 1, 0),
(15, 'medhitest', 'admin@heolia.eu', '$2y$10$U9VzwXEllEdjrqBn1O7iIuv/CBlAIxLnaWEHcik55RkzfmIjmMZUK', NULL, 'vqezjvyw-nxki-4rsu-artq-kj0zy3pumvoi', 0, 1, 0),
(16, 'toto', 'toto@titi.com', '$2y$10$mI6LXP67yD.QxYcroNUrC.L5uSFIDvFNhbxnxLR6T5hYis5Da5b8O', NULL, 'zw3wf7w2-8gia-ymmj-vw71-3k3he9pbh8og', 0, 1, 0),
(17, 'tata', 'tata@tata.com', '$2y$10$0ZOMzrRQcpMpQNrH8smmkOtFUXb1mKJMXgNMf1gXND0PxTF2XEgv2', NULL, 'akp4wqf5-umyi-4400-tanz-7lzlbq7l677h', 0, 1, 0);

-- --------------------------------------------------------

--
-- Structure de la table `room_list`
--

CREATE TABLE `room_list` (
  `id` int(11) NOT NULL,
  `name` varchar(20) DEFAULT NULL,
  `current_players` int(11) DEFAULT NULL,
  `max_players` int(11) DEFAULT NULL,
  `creator_user_id` char(36) CHARACTER SET utf8mb4 DEFAULT NULL,
  `list_players_room` int(11) NOT NULL,
  `statement` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Structure de la table `room_list_players`
--

CREATE TABLE `room_list_players` (
  `id` int(11) NOT NULL,
  `room_list` int(11) NOT NULL,
  `player_id` int(11) DEFAULT NULL,
  `isConnected` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Index pour les tables exportées
--

--
-- Index pour la table `account`
--
ALTER TABLE `account`
  ADD PRIMARY KEY (`id`,`name`,`email`,`password`,`userid`),
  ADD UNIQUE KEY `name` (`name`),
  ADD UNIQUE KEY `id` (`id`),
  ADD UNIQUE KEY `email` (`email`),
  ADD UNIQUE KEY `userid` (`userid`),
  ADD KEY `userid_2` (`userid`);

--
-- Index pour la table `room_list`
--
ALTER TABLE `room_list`
  ADD PRIMARY KEY (`id`),
  ADD KEY `list_players_room` (`list_players_room`),
  ADD KEY `creator_user_id` (`creator_user_id`);

--
-- Index pour la table `room_list_players`
--
ALTER TABLE `room_list_players`
  ADD PRIMARY KEY (`id`),
  ADD KEY `room_list` (`room_list`),
  ADD KEY `player_id` (`player_id`);

--
-- AUTO_INCREMENT pour les tables exportées
--

--
-- AUTO_INCREMENT pour la table `account`
--
ALTER TABLE `account`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;
--
-- AUTO_INCREMENT pour la table `room_list`
--
ALTER TABLE `room_list`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT pour la table `room_list_players`
--
ALTER TABLE `room_list_players`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- Contraintes pour les tables exportées
--

--
-- Contraintes pour la table `room_list`
--
ALTER TABLE `room_list`
  ADD CONSTRAINT `creator_user_id` FOREIGN KEY (`creator_user_id`) REFERENCES `account` (`userid`),
  ADD CONSTRAINT `room_list_ibfk_1` FOREIGN KEY (`list_players_room`) REFERENCES `room_list_players` (`room_list`);

--
-- Contraintes pour la table `room_list_players`
--
ALTER TABLE `room_list_players`
  ADD CONSTRAINT `room_list_players_ibfk_1` FOREIGN KEY (`player_id`) REFERENCES `account` (`id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
