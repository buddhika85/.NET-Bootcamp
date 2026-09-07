import React from 'react';
import { useLocation } from 'react-router-dom';

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    nameSearch?: string;
}

const Pagination: React.FC<PaginationProps> = ({ currentPage, totalPages, nameSearch }) => {
    const location = useLocation();

    const buildUrl = (page: number): string => {
        const params = new URLSearchParams();
        if (page > 1) {
            params.set('page', page.toString());
        }
        if (nameSearch) {
            params.set('name', nameSearch);
        }
        const query = params.toString();
        return `${location.pathname}${query ? `?${query}` : ''}`;
    };

    const pageNumbers = Array.from({ length: totalPages }, (_, i) => i + 1);

    return (
        <nav>
            <ul className="pagination justify-content-center">
                <li className={`page-item ${currentPage <= 1 ? 'disabled' : ''}`}>
                    <a className="page-link" href={buildUrl(currentPage - 1)}>Previous</a>
                </li>

                {pageNumbers.map((page) => (
                    <li key={page} className={`page-item ${page === currentPage ? 'active' : ''}`}>
                        <a className="page-link" href={buildUrl(page)}>{page}</a>
                    </li>
                ))}

                <li className={`page-item ${currentPage >= totalPages ? 'disabled' : ''}`}>
                    <a className="page-link" href={buildUrl(currentPage + 1)}>Next</a>
                </li>
            </ul>
        </nav>
    );
};

export default Pagination;
