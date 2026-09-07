import React, { useState, useEffect, useRef } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import GamesClient from '../clients/GamesClient';
import { GamesPage } from '../models/GamesPage';
import { GameSummary } from '../models/GameSummary';
import Pagination from '../components/Pagination';

const PAGE_SIZE = 10;

const GameCard: React.FC<{ game: GameSummary }> = ({ game }) => {
    const [hovered, setHovered] = useState(false);

    return (
        <div className="col mt-4">
            <Link to={`/game/${game.id}`} style={{ textDecoration: 'none' }}>
                <div
                    className="card h-100"
                    style={{
                        transition: 'box-shadow 0.2s ease-in-out',
                        boxShadow: hovered ? '0 0 10px #5f4dee' : undefined,
                    }}
                    onMouseEnter={() => setHovered(true)}
                    onMouseLeave={() => setHovered(false)}
                >
                    <img src={game.imageUri} className="card-img-top" alt={game.name} />
                    <div className="card-body">
                        <h5 className="card-title">{game.name}</h5>
                        <p className="card-text">${game.price}</p>
                    </div>
                </div>
            </Link>
        </div>
    );
};

const Home: React.FC = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [gamesPage, setGamesPage] = useState<GamesPage | null>(null);
    const [loadingError, setLoadingError] = useState<string | null>(null);
    const searchInputRef = useRef<HTMLInputElement>(null);

    const currentPage = parseInt(searchParams.get('page') ?? '1', 10);
    const nameFilter = searchParams.get('name') ?? undefined;

    useEffect(() => {
        document.title = 'Game Store';
        setGamesPage(null);
        setLoadingError(null);

        const fetchGames = async () => {
            try {
                const client = new GamesClient();
                const data = await client.getGamesAsync(currentPage, PAGE_SIZE, nameFilter);
                setGamesPage(data);
            } catch (error: unknown) {
                if (error instanceof Error) {
                    setLoadingError(error.message);
                } else {
                    setLoadingError('An unknown error occurred');
                }
            }
        };

        fetchGames();
    }, [currentPage, nameFilter]);

    const handleSearch = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const term = searchInputRef.current?.value.trim() ?? '';
        const next = new URLSearchParams();
        if (term) {
            next.set('name', term);
        }
        setSearchParams(next);
    };

    return (
        <div>
            <div className="row mt-2">
                <div className="col-sm-4">
                    <form className="d-flex" role="search" onSubmit={handleSearch}>
                        <input
                            ref={searchInputRef}
                            className="form-control me-2"
                            type="search"
                            defaultValue={nameFilter ?? ''}
                            placeholder="Search store"
                            aria-label="Search"
                        />
                        <button className="btn btn-outline-primary" type="submit">Search</button>
                    </form>
                </div>
            </div>

            {loadingError && (
                <div className="mt-3 alert alert-danger">{loadingError}</div>
            )}

            {!gamesPage && !loadingError && (
                <p className="mt-3"><em>Loading...</em></p>
            )}

            {gamesPage && (
                <>
                    <div className="row row-cols-1 row-cols-md-5 mt-3">
                        {gamesPage.data.map((game) => (
                            <GameCard key={game.id} game={game} />
                        ))}
                    </div>

                    <div className="row mt-2">
                        <div className="col">
                            <Pagination
                                currentPage={currentPage}
                                totalPages={gamesPage.totalPages}
                                nameSearch={nameFilter}
                            />
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};

export default Home;
