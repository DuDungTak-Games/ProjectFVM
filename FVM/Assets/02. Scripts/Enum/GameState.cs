public enum GameState
{
    LOADING, // 로딩
    
    LOBBY, // 로비

    COIN_COLLECTION, // 코인 수집
    
    LAUNCH_PREPARE, // 자판기 상품 구매
    LAUNCH_READY, // 자판기 발사 준비
    LAUNCH_PLAYING, // 자판기 발사
    
    END, // 자판기 발사 종료
}