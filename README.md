# C#luedo - un jeu CLI

C#luedo est un jeu textuel, l'interface étant l'invite de commandes de Windows.
Notre application s'inspire du célèbre jeu "Cluedo", ici destiné à un seul joueur. Le personnage interprété par le joueur est un inspecteur qui doit trouver qui a tué le Docteur Lenoir et avec quelle arme a été commis le meurtre.

![Visualisation 3D du manoir](https://raw.githubusercontent.com/fabienmonniot/C-luedo/master/manoir.png)

Six personnes sont suspectées d'avoir commis le meurtre, et sont présentes dans six pièces différentes du manoir. Six armes sont également réparties dans le manoir. Les pièces contiennent des objets qu'il est possible d'observer pour en apprendre davantage, ainsi que des passages possibles entre elles par des portes ouvertes. Certains déplacements peuvent s'ajouter via des passages secrets.

Selon la pièce dans laquelle se trouve le joueur et son contenu (un suspect et/ou une arme ou aucun des deux), différentes actions s'offrent à lui. Chaque type d'action prend un certain temps à être effectué, et l’inspecteur a un temps limité pour résoudre l'affaire.

L'inspecteur est doté d'un inventaire dans lequel il peut déposer certains objets. Dès le début et tout au long de la partie, un bloc-notes est présent dans son inventaire, dans lequel il écrit au fur et à mesure de l'enquête les informations qu’il apprend.

## Scénario

L'inspecteur arrive le matin à 8h dans le jardin du manoir. Il est reçu par un policier qui lui rappelle les conditions du meurtre et lui présente succinctement chacun des suspects, puis lui remet un talkie-walkie permettant au policier de contacter l’inspecteur.

L'inspecteur rentre ensuite dans le hall du manoir et commence à mener seul son enquête. Le policier reste tout le long de la partie dans le jardin. L'inspecteur a en tout 10h pour résoudre l'affaire : de 8h à 18h.

Au bout de 4 heures d'enquête, la fille du docteur arrive sur les lieux, elle se place dans le jardin avec le policier et le joueur peut aller la voir pour manger avec elle afin d'obtenir des informations supplémentaires. Il a cependant un temps limité à une heure pour saisir cette opportunité. Après, la fille du docteur repart et n'est plus disponible pour le reste de la partie.

À tout moment de la partie, l'inspecteur peut accuser un suspect, en précisant l'arme avec laquelle il pense que le meurtre a été commis. Pour cela, il doit retourner dans le jardin où se trouve le policier afin de lui adresser son hypothèse. 
Si l'accusation est juste la partie est gagnée, sinon elle est perdue. Le joueur peut alors choisir de découvrir la bonne réponse ou de rejouer.

A 18h au plus tard, la partie est terminée. Si l'inspecteur n’est pas encore retourné dans le jardin pour accuser un des suspects et donner l'arme du crime, le policier le joint à l’aide du talkie walkie et lui demande son verdict final.

## Téléchargement

C#luedo est un exécutable Windows. Il a été testé sur Windows 7 et 10.
Vous pouvez le télécharger ici : ![https://github.com/fabienmonniot/C-luedo](https://github.com/fabienmonniot/C-luedo)

Conçu et développé par Louise Mathieu & Fabien Monniot
