import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import GamesClient from '../clients/GamesClient';
import { GameDetails } from '../models/GameDetails';

const Game: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [game, setGame] = useState<GameDetails | null>(null);
    const [errorList, setErrorList] = useState<string[]>([]);

    useEffect(() => {
        const fetchGame = async () => {
            if (!id) return;
            try {
                const client = new GamesClient();
                const data = await client.getGameAsync(id);
                setGame(data);
                document.title = data.name;
            } catch (error: unknown) {
                if (error instanceof Error) {
                    setErrorList([error.message]);
                } else {
                    setErrorList(['An unknown error occurred']);
                }
            }
        };
        fetchGame();
    }, [id]);

    const formatDate = (dateStr: string): string => {
        const date = new Date(`${dateStr}T00:00:00`);
        return date.toLocaleDateString('en-US', {
            month: 'short',
            day: '2-digit',
            year: 'numeric',
        });
    };

    if (errorList.length > 0) {
        return (
            <div className="mt-3">
                {errorList.map((error, index) => (
                    <div key={index} className="alert alert-danger">{error}</div>
                ))}
            </div>
        );
    }

    if (!game) {
        return <p className="mt-3"><em>Loading...</em></p>;
    }

    return (
        <div className="row mt-4">
            <div className="col-md-4">
                <img
                    src={game.imageUri ?? ''}
                    alt={game.name}
                    className="img-fluid border border-secondary"
                />
            </div>
            <div className="col-md-5">
                <h2>{game.name}</h2>
                <p className="mt-3">{game.description}</p>
                <p className="display-4 fw-bold">${game.price}</p>
                <p className="text-secondary mt-3">Release Date: {formatDate(game.releaseDate)}</p>
            </div>
        </div>
    );
};

export default Game;
