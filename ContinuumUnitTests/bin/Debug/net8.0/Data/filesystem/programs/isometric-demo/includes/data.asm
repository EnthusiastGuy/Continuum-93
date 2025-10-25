; This holds some data and reserved space for tile sheets
; It should be clear that changing the size of the tilesheets MUST absolutely
; also reflect in changing the reserved sizes below
; Always define the size of the buffers for data you load, otherwise include
; this file LAST so it will be able to use any free memory without overwriting
; some other code included after it.
.PathTilesA
    #DB "programs\isometric-demo\img\tilesA.png", 0

.PathTilesB
    #DB "programs\isometric-demo\img\tilesB.png", 0

.PathBackground
    #DB "programs\isometric-demo\img\background.png", 0

.TilesAData
    #DB [6144] 0          ; Reserving space for the first page of tiles (128px X 48px)

.TilesBData
    #DB [6144] 0          ; Reserving space for the second page of tiles (128px X 48px)

.Layer0Palette
    #DB 0x000000
