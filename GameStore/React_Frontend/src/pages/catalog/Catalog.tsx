import React, { useState, useEffect } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import GamesClient from '../../clients/GamesClient';
import { GamesPage } from '../../models/GamesPage';
import { GameSummary } from '../../models/GameSummary';
import DeleteGameModal from '../../components/DeleteGameModal';
import Pagination from '../../components/Pagination';

// Declare bootstrap property on window object
declare global {
    interface Window {
        bootstrap: any;
    }
}

const PAGE_SIZE = 5;

const Catalog: React.FC = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [gamesPage, setGamesPage] = useState<GamesPage | null>(null);
    const [loadingErrorList, setLoadingErrorList] = useState<string[]>([]);
    const [errorList, setErrorList] = useState<string[]>([]);
    const [gameToDelete, setGameToDelete] = useState<GameSummary | null>(null);

    const currentPage = parseInt(searchParams.get('page') ?? '1', 10);
    const nameFilter = searchParams.get('name') ?? undefined;

    const fetchGames = async () => {
        setLoadingErrorList([]);
        try {
            const gamesClient = new GamesClient();
            const data = await gamesClient.getGamesAsync(currentPage, PAGE_SIZE, nameFilter);
            setGamesPage(data);
        } catch (error: unknown) {
            if (error instanceof Error) {
                setLoadingErrorList([error.message]);
            } else {
                setLoadingErrorList(['An unknown error occurred']);
            }
        }
    };

    useEffect(() => {
        document.title = 'Game Catalog';
        fetchGames();
    }, [currentPage, nameFilter]);

    useEffect(() => {
        if (gameToDelete) {
            const modalEl = document.getElementById(`deleteModal-${gameToDelete.id}`)!;
            const modal = new window.bootstrap.Modal(modalEl);
            const handleHidden = () => setGameToDelete(null);
            modalEl.addEventListener('hidden.bs.modal', handleHidden);
            modal.show();
            return () => modalEl.removeEventListener('hidden.bs.modal', handleHidden);
        }
    }, [gameToDelete]);

    const handleDelete = async (gameId: string) => {
        setErrorList([]);
        try {
            const gamesClient = new GamesClient();
            const result = await gamesClient.deleteGameAsync(gameId);

            if (result.succeeded) {
                fetchGames();
            } else {
                setErrorList(result.errors);
            }
        } catch (error: unknown) {
            if (error instanceof Error) {
                setErrorList([error.message]);
            } else {
                setErrorList(['An unknown error occurred']);
            }
        }
    };

    const handleSearch = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const form = event.currentTarget;
        const term = (form.elements.namedItem('nameSearch') as HTMLInputElement).value.trim();
        const next = new URLSearchParams();
        if (term) next.set('name', term);
        setSearchParams(next);
    };

    if (loadingErrorList.length > 0) {
        return (
            <div>
                {loadingErrorList.map((error, index) => (
                    <div key={index} className="mt-3 text-danger">
                        <em>{error}</em>
                    </div>
                ))}
            </div>
        );
    }

    if (gamesPage === null) {
        return <p className="mt-3"><em>Loading...</em></p>;
    }

    return (
        <div>
            <div className="row mt-2">
                <div className="col">
                    <Link className="btn btn-primary" to="/catalog/editgame" role="button">
                        New Game
                    </Link>
                </div>
                <div className="col-sm-4">
                    <form className="d-flex" role="search" onSubmit={handleSearch}>
                        <input
                            name="nameSearch"
                            className="form-control me-2"
                            type="search"
                            defaultValue={nameFilter ?? ''}
                            placeholder="Search..."
                            aria-label="Search"
                        />
                        <button className="btn btn-outline-primary" type="submit">Search</button>
                    </form>
                </div>
            </div>

            {errorList.length > 0 && (
                <div className="modal-body mt-3">
                    {errorList.map((error, index) => (
                        <div key={index} className="alert alert-danger">
                            {error}
                        </div>
                    ))}
                </div>
            )}

            <table className="table table-striped table-bordered table-hover mt-3">
                <thead className="table-dark">
                    <tr>
                        <th>Image</th>
                        <th>Name</th>
                        <th>Genre</th>
                        <th className="text-end">Price</th>
                        <th>Release Date</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {gamesPage.data.map((game) => (
                        <tr key={game.id}>
                            <td style={{ width: '60px', maxWidth: '60px' }}>
                                <img src={game.imageUri} alt={game.name} style={{ width: '50px', objectFit: 'contain' }} />
                            </td>
                            <td>{game.name}</td>
                            <td>{game.genre}</td>
                            <td className="text-end">${game.price}</td>
                            <td>{game.releaseDate}</td>
                            <td>
                                <div className="d-flex">
                                    <Link className="btn btn-primary me-2" to={`/catalog/editgame/${game.id}`} role="button">
                                        <i className="bi bi-pencil"></i>
                                    </Link>
                                    <button className="btn btn-danger" onClick={() => setGameToDelete(game)}>
                                        <i className="bi bi-x-lg"></i>
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="row mt-2">
                <div className="col">
                    <Pagination
                        currentPage={currentPage}
                        totalPages={gamesPage.totalPages}
                        nameSearch={nameFilter}
                    />
                </div>
            </div>

            {/* Delete Confirmation Modal */}
            {gameToDelete && (
                <DeleteGameModal game={gameToDelete} onDelete={handleDelete} />
            )}
        </div>
    );
};

export default Catalog;
