# Instructions d'installation - Écran de début

## Étape 1 : Créer l'interface utilisateur (UI Canvas)

1. **Créer un Canvas :**

   - Dans Unity, clic droit dans la hiérarchie → UI → Canvas
   - Renommer en "StartScreenCanvas"
   - Dans le Canvas Scaler : mettre "UI Scale Mode" à "Scale With Screen Size"
   - Reference Resolution : 1920x1080

2. **Créer le texte d'introduction :**

   - Clic droit sur StartScreenCanvas → UI → Text - TextMeshPro
   - Si c'est la première fois : cliquer "Import TMP Essentials"
   - Renommer en "IntroText"
   - **Position :**
     - Anchor Presets : cliquer sur le carré en haut à gauche, maintenir ALT+SHIFT et cliquer sur "stretch both"
     - Left : 100
     - Right : 100
     - Top : 200
     - Bottom : 300
   - **Paramètres du texte :**
     - Font Size : 36
     - Color : Blanc (255, 255, 255, 255)
     - Alignment : Center et Middle
     - Wrapping : activé
     - Le champ "Text" doit être VIDE (le script le remplira)

3. **Créer le bouton "COMMENCER" :**
   - Clic droit sur StartScreenCanvas → UI → Button - TextMeshPro
   - Renommer en "StartButton"
   - **Position :**
     - Anchor Presets : Bottom Center
     - Pos X : 0
     - Pos Y : 100
     - Width : 300
     - Height : 60
   - **Paramètres du bouton :**
     - Colors : Normal Color = blanc, Highlighted = gris clair, Pressed = gris foncé
   - **Texte du bouton :**
     - Sélectionner l'enfant "Text (TMP)" dans StartButton
     - Text : "COMMENCER"
     - Font Size : 28
     - Color : Blanc
     - Alignment : Center et Middle

## Étape 2 : Configurer le StartScreen Script

1. **Ajouter le script :**

   - Créer un GameObject vide : Clic droit dans la hiérarchie → Create Empty
   - Renommer en "StartScreenManager"
   - Glisser le script `StartScreen.cs` dessus

2. **Assigner les références :**
   - Sélectionner "StartScreenManager"
   - Dans l'Inspector :

     - **UI Elements :**

       - Intro Text : glisser "IntroText" depuis la hiérarchie
       - Start Button : glisser "StartButton" depuis la hiérarchie
       - Button Text : glisser "StartButton/Text (TMP)" depuis la hiérarchie

     - **Camera Settings :**

       - Player Camera : glisser votre Main Camera
       - Monster Eyes : créer un point pour les yeux du monstre (voir étape 3)
       - Monster : glisser votre monstre depuis la hiérarchie

     - **Text Settings :**

       - Text Lines : Laisser par défaut ou personnaliser
       - Typewriter Speed : 0.05 (ajuster selon préférence)
       - Delay Between Lines : 0.8

     - **Player References :**
       - Player : glisser votre GameObject Player
       - Monster AI : glisser le monstre qui a le script MonsterAI

## Étape 3 : Créer le point "Monster Eyes"

1. Sélectionner votre monstre dans la hiérarchie
2. Clic droit → Create Empty
3. Renommer en "MonsterEyes"
4. Positionner à l'endroit des yeux du monstre (en général Y autour de 1.5-2m)
5. Glisser ce "MonsterEyes" dans le champ "Monster Eyes" du StartScreen script

## Étape 4 : Configurer le GameManager

1. **Créer le GameManager :**

   - Clic droit dans la hiérarchie → Create Empty
   - Renommer en "GameManager"
   - Glisser le script `GameManager.cs` dessus

2. **Assigner les références :**
   - Player : votre GameObject Player
   - Monster AI : votre monstre avec le script MonsterAI
   - Portal : votre portail (à créer plus tard si besoin)
   - Portal Distance To Win : 3 (distance pour gagner)

## Étape 5 : Configuration du monstre

1. **S'assurer que le monstre est visible au début :**

   - Positionner le monstre en face du joueur au début de la scène
   - Le monstre doit être à une distance raisonnable (5-10 mètres)
   - Le monstre doit faire face au joueur

2. **Vérifier le script MonsterAI :**
   - Le script a été modifié pour utiliser le GameManager
   - Il sera désactivé au début et activé quand le joueur clique sur "COMMENCER"

## Étape 6 : Tags et Layers (Important !)

1. **Tag "Player" :**

   - Sélectionner votre GameObject Player
   - En haut de l'Inspector : Tag → Player

2. **Tag "Portal" (optionnel pour plus tard) :**
   - Créer un tag "Portal" : Edit → Project Settings → Tags and Layers
   - Ajouter "Portal" dans les Tags
   - Assigner ce tag à votre portail quand vous le créerez

## Étape 7 : Tester

1. **Lancer le jeu en mode Play**
2. **Ce qui devrait se passer :**
   - La caméra regarde fixement les yeux du monstre
   - Le monstre est complètement figé
   - Le texte s'affiche lettre par lettre
   - Le bouton "COMMENCER" apparaît après le texte
   - Le curseur devient visible pour cliquer
   - En cliquant sur "COMMENCER" : le jeu démarre, le joueur peut bouger, le monstre s'active

## Troubleshooting

### Le texte ne s'affiche pas :

- Vérifier que le champ "Text" de IntroText est vide dans Unity
- Vérifier que "Intro Text" est bien assigné dans le script

### Le bouton ne fonctionne pas :

- Vérifier que EventSystem existe dans la hiérarchie (créé automatiquement avec le Canvas)
- Vérifier que "Start Button" est bien assigné

### Le monstre bouge au début :

- Vérifier que le script MonsterAI est bien assigné dans StartScreen
- Le script sera désactivé automatiquement

### La caméra ne regarde pas le monstre :

- Vérifier que "Monster Eyes" est bien positionné sur les yeux du monstre
- Vérifier que "Player Camera" et "Monster Eyes" sont assignés

### Le joueur peut bouger au début :

- Le script désactive automatiquement tous les scripts de type "FirstPerson", "Player", "Controller"
- Si ça ne marche pas, vérifier les noms de vos scripts de contrôle

## Personnalisation

### Changer le texte :

- Dans StartScreenManager → Text Lines, modifier les lignes

### Changer les couleurs :

- Sélectionner IntroText ou StartButton/Text (TMP)
- Modifier "Color" dans les paramètres

### Changer la vitesse du typewriter :

- Dans StartScreenManager → Typewriter Speed (plus petit = plus rapide)

### Changer le volume du son ambiant :

- Les sons d'ambiance continuent de jouer pendant l'écran de début
- Ajuster dans votre HorrorSoundManager si nécessaire

## Note importante

Le portail n'est pas encore créé. Vous devrez :

1. Créer un objet "Portal" dans votre scène
2. Lui donner le tag "Portal"
3. L'assigner dans le GameManager
4. Le joueur devra trouver ce portail pour gagner (tout en ne perdant pas le monstre de vue !)
