import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import GamesClient from '../../clients/GamesClient';
import { GameSummary } from '../../models/GameSummary';
import DeleteGameModal from '../../components/DeleteGameModal';

// Declare bootstrap property on window object
declare global {
    interface Window {
        bootstrap: any;
    }
}

const Catalog: React.FC = () => {
    const [games, setGames] = useState<GameSummary[] | null>(null);
    const [loadingErrorList, setLoadingErrorList] = useState<string[]>([]);
    const [errorList, setErrorList] = useState<string[]>([]);
    const [gameToDelete, setGameToDelete] = useState<GameSummary | null>(null);

    const fetchGames = async () => {
        try {
            const gamesClient = new GamesClient();
            const data = await gamesClient.getGamesAsync();
            setGames(data);
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
    }, []);

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

    if (loadingErrorList.length > 0) {
        return <div>
            {loadingErrorList.map((error, index) => (
                <div key={index} className="mt-3 text-danger">
                    <em>{error}</em>
                </div>
            ))}
        </div>;
    }

    if (games === null) {
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
                        <th>Name</th>
                        <th>Genre</th>
                        <th className="text-end">Price</th>
                        <th>Release Date</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {games.map((game) => (
                        <tr key={game.id}>
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

            {/* Delete Confirmation Modal */}
            {gameToDelete && (
                <DeleteGameModal game={gameToDelete} onDelete={handleDelete} />
            )}
        </div>
    );
};

export default Catalog;
