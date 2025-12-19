# Instructions - Écran de Game Over

## Étape 1 : Créer l'interface Game Over

### 1. Créer un Panel pour le Game Over

1. Dans la hiérarchie, sélectionner votre **StartScreenCanvas** (ou créer un nouveau Canvas)
2. Clic droit sur le Canvas → UI → Panel
3. Renommer en **"GameOverPanel"**
4. **Configuration du Panel :**
   - Dans le Rect Transform : cliquer sur Anchor Presets (carré en haut à gauche)
   - Maintenir ALT+SHIFT et cliquer sur "stretch both" (en bas à droite)
   - Mettre tous les paramètres (Left, Right, Top, Bottom) à **0**
   - Dans Image (Script) :
     * Color : Noir avec alpha à 0 (complètement transparent)
     * Ou si vous voulez un léger assombrissement : alpha à 50-100

### 2. Créer le texte "GAME OVER"

1. Clic droit sur GameOverPanel → UI → Text - TextMeshPro
2. Renommer en **"GameOverText"**
3. **Configuration :**
   - Anchor Presets : Top Center
   - Pos Y : -200 (pour le mettre en haut mais pas tout en haut)
   - Width : 800
   - Height : 100
   - **Paramètres du texte :**
     * Text : "GAME OVER"
     * Font Size : 72 (très gros)
     * Color : Blanc (255, 255, 255, 255)
     * Alignment : Center et Middle
     * Font Style : Bold (si disponible)

### 3. Créer le message d'avertissement

1. Clic droit sur GameOverPanel → UI → Text - TextMeshPro
2. Renommer en **"MessageText"**
3. **Configuration :**
   - Anchor Presets : Center
   - Pos Y : 0 (au centre de l'écran)
   - Width : 900
   - Height : 150
   - **Paramètres du texte :**
     * Text : "Il ne fallait pas me perdre de vue"
     * Font Size : 36
     * Color : Blanc (255, 255, 255, 255)
     * Alignment : Center et Middle
     * Wrapping : activé

### 4. Créer le bouton "RECOMMENCER"

1. Clic droit sur GameOverPanel → UI → Button - TextMeshPro
2. Renommer en **"RestartButton"**
3. **Configuration :**
   - Anchor Presets : Bottom Center
   - Pos Y : 150 (en bas mais pas tout en bas)
   - Width : 300
   - Height : 60
   - **Paramètres du bouton :**
     * Colors : Normal = blanc, Highlighted = gris clair, Pressed = gris foncé
   - **Texte du bouton :**
     * Sélectionner l'enfant "Text (TMP)"
     * Text : "RECOMMENCER"
     * Font Size : 28
     * Color : Blanc
     * Alignment : Center et Middle

## Étape 2 : Configurer le script GameOverScreen

1. **Ajouter le script au GameManager :**
   - Sélectionner votre GameObject "GameManager" dans la hiérarchie
   - Glisser le script `GameOverScreen.cs` dessus
   - (Le GameManager aura maintenant 2 scripts : GameManager et GameOverScreen)

2. **Assigner les références :**
   - Sélectionner "GameManager"
   - Dans le composant "Game Over Screen (Script)" :
     * **UI Elements :**
       - Game Over Panel : glisser "GameOverPanel"
       - Game Over Text : glisser "GameOverText"
       - Message Text : glisser "MessageText"
       - Restart Button : glisser "RestartButton"
       - Restart Button Text : glisser "RestartButton/Text (TMP)"
     
     * **Animation Settings :**
       - Text Fade In Duration : 1.0 (ajustable)
       - Delay Before Button : 1.5 (temps avant que le bouton apparaisse)

3. **Lier le GameOverScreen au GameManager :**
   - Toujours sur le GameObject "GameManager"
   - Dans le composant "Game Manager (Script)" :
     * Game Over Screen : glisser le même GameObject "GameManager" (car il contient le script GameOverScreen)
     * OU laisser vide, le script le trouvera automatiquement

## Étape 3 : Vérification

### Ce qui devrait se passer quand le monstre attrape le joueur :

1. L'image se fige (le joueur voit encore le monstre)
2. Le texte "GAME OVER" apparaît progressivement en haut (fade in)
3. Le message "Il ne fallait pas me perdre de vue" apparaît juste après
4. Après 1.5 secondes, le bouton "RECOMMENCER" apparaît
5. Le joueur peut cliquer sur "RECOMMENCER" pour relancer la partie

### Tester :

1. Lancer le jeu en mode Play
2. Se faire attraper par le monstre (le laisser vous atteindre)
3. Vérifier que l'écran de Game Over s'affiche correctement
4. Cliquer sur "RECOMMENCER" pour vérifier que la scène se recharge

## Troubleshooting

### L'écran de Game Over ne s'affiche pas :
- Vérifier que "GameOverPanel" est bien assigné dans GameOverScreen
- Vérifier que GameOverScreen est bien assigné dans GameManager
- Vérifier dans la Console s'il y a des erreurs

### Le texte n'apparaît pas :
- Vérifier que "Game Over Text" et "Message Text" sont assignés
- Vérifier que la couleur est bien blanche (pas noire sur fond noir)
- Vérifier que le Canvas est en mode "Screen Space - Overlay"

### Le bouton ne fonctionne pas :
- Vérifier que "Restart Button" est bien assigné
- Vérifier qu'il y a un EventSystem dans la scène
- Vérifier que le bouton a bien un composant Button

### L'image n'est pas figée :
- C'est normal si Time.timeScale est à 1
- L'écran s'affiche par-dessus le jeu qui continue
- Si vous voulez figer complètement, vous pouvez ajouter `Time.timeScale = 0;` dans ShowGameOver()
- ATTENTION : si vous faites ça, il faudra modifier GameOverScreen pour utiliser `Time.unscaledDeltaTime` au lieu de `Time.deltaTime`

### Le curseur n'apparaît pas :
- Le script déverrouille automatiquement le curseur
- Vérifier dans GameOverScreen.ShowGameOver() que `Cursor.visible = true`

## Personnalisation

### Changer les textes :
- Sélectionner "GameOverText" ou "MessageText"
- Modifier le champ "Text" dans l'Inspector

### Changer les couleurs :
- Sélectionner les textes ou le bouton
- Modifier "Color" dans les paramètres

### Changer la vitesse d'animation :
- Dans GameManager → Game Over Screen (Script)
- Ajuster "Text Fade In Duration" (plus petit = plus rapide)
- Ajuster "Delay Before Button" (temps avant que le bouton apparaisse)

### Ajouter un fond sombre :
- Sélectionner "GameOverPanel"
- Dans Image (Script) → Color : mettre alpha entre 100-200
- Cela assombrira l'image derrière

### Ajouter un bouton "QUITTER" :
1. Dupliquer le bouton "RECOMMENCER"
2. Renommer en "QuitButton"
3. Changer le texte en "QUITTER"
4. Dans l'événement OnClick : sélectionner GameManager → GameOverScreen.QuitGame()

## Notes importantes

- L'écran de Game Over s'affiche automatiquement quand le monstre attrape le joueur
- Le GameManager gère l'appel à `ShowGameOver()`
- Le MonsterAI appelle `GameManager.Instance.GameOver()` quand il touche le joueur
- La scène se recharge complètement quand on clique sur "RECOMMENCER"
