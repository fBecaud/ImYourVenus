# TP 

## Objectif

L’objectif de ce TP était de créer un simulateur de système solaire, ou de manière plus générale d’un ensemble de N corps célestes. 
On s’intéresse donc aux champs de forces gravitationnelles qui sont des champs vectoriels conservatifs.

## Features

Choix de :
 - La position de la caméra ([Free Fly Cam par Sergey Stafeev](https://assetstore.unity.com/packages/tools/camera/free-fly-camera-140739) : 
 WASD mouvements, QE monter/descendre, souris pour l'orientation, shift boost vitesse, accélération).
 - Le placement, la masse et la vitesse initiale d'une nouvelle planète dans le système.
 - La vitesse de la simution (en X par Ticks, ex: 120 heures par game tick).
 - Monter ou cacher certains éléments: les informations d'une planète (vitesse, masse, position), le champ vectoriel.  

## Précisions

Les constantes utilisées :
 - Gravite Universel = 6.6743e-11F

Valeurs conseillées :
 - Vitesse de la simution : Plus la vitesse est rapide, plus les imprécisions s'accumulent (et plus les trails des planètes sont grosses).
 On recommande donc de rester sur des valeurs proche de celle par défauts. 
 - Nouvelle planète : 

## Architecture 

Tous les calculs des planètes sont trouvables dans Astral Object.
Les constantes et calculs du temps dans Globals.
Le champ vectoriel dans Vectorial Fields.
La camera dans Camera Behaviour & Free Fly Camera.
Les interfaces dans Instruction Button, New Planet Placer, Planet Info & Time Controller.

## Images

![SC4](Screens/ToInfinityAndBeyond.png)
_Gravitation follows the Sun if moving._
![SC1](Screens/VectorialField.png)
_Display of vectorial field._
![SC2](Screens/Rotational.png)
_Display of rotational field._
![SC3](Screens/FieldofLines.png.png)
_Display of field of lines._
![SC5](Screens/AsteroidField.png)
_Second scene with an asteroid field._


## Écrit par
- <a href = "mailto: f.becaud@student.isartdigital.com">Félix Becaud</a>
- <a href = "mailto: j.perrochaud@student.isartdigital.com">Jessica Perrochaud</a>
- <a href = "mailto: m.dero@student.isartdigital.com">Morgane Dero</a>
