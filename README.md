# SuiviFormation
Application en Blazor pour faire le suivi des formations d'un service, d'une équipe.
J'ai fait cette application pour mon service. Il voulait une application qui permettait de suivre les différentes formations que les personnels avaient suivis. Quand ? Avec quel formateur ? Ont-ils validés la formation ?<br/>La contrainte était sur la base de donnée, __que__ sur MySQL. Le schéma de base de donnée est fournis dans SuiviFormation/Script.

## Lancement de l'application ##
Au lancement de l'application, je crée l'utilisateur "root", avec le rôle administrateur.
* login : root
* mdp : Azerty123!

Je me base sur les autorisations sur les rôles :
* **Rôle gestionnaire** : il permet de créer des salles, des formations dans le catalogue, de nouvelle session de formation.
* **Rôle administrateur** : il peut faire comme le rôle gestionnaire, en plus il peut changer les rôles des utilisateurs.

## Ajout d'une formation au catalogue ##
Il faut plusieurs informations :
* un titre
* un temps de formation en jour, mais cela peut être 0.5, pour une demi journée
* une petite description de la formation
* s'il y a une date de fin, c'est que cette formation ne sera plus dispensé à cette date
* un fichier de contenu, c'est libre, mais donne les grandes lignes du contenu.

## Création d'une session ##
Pour créer une session de formation, il faut fournir :
* une formation du catalogue
* une salle ou aura lieu cette formation
* un formateur
Ensuite se sont les agents qui viennent s'inscrire à la session. Il est possible d'éditer la session et d'inscrire un personnel à cette session.
