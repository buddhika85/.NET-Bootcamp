// src/components/Pagination.tsx
import React from 'react';
import type { PaginationInfo } from '../models/PaginationInfo';

interface PaginationProps {
  paginationInfo: PaginationInfo;
  onPageChange: (pageNumber: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({ paginationInfo, onPageChange }) => {
  const getPageNumbers = (paginationInfo: PaginationInfo) => {
    const pageNumbers = [];
    for (let i = 1; i <= paginationInfo.totalPages; i++) {
      pageNumbers.push(i);
    }
    return pageNumbers;
  };

  return (
    <nav>
      <ul className="pagination justify-content-center">
        <li className={`page-item ${!paginationInfo.hasPrevious ? 'disabled' : ''}`}>
          <button className="page-link" onClick={() => onPageChange(paginationInfo.currentPage - 1)} disabled={!paginationInfo.hasPrevious}>
            Previous
          </button>
        </li>

        {getPageNumbers(paginationInfo).map((pageNumber) => (
          <li key={pageNumber} className={`page-item ${pageNumber === paginationInfo.currentPage ? 'active' : ''}`}>
            <button className="page-link" onClick={() => onPageChange(pageNumber)}>
              {pageNumber}
            </button>
          </li>
        ))}

        <li className={`page-item ${!paginationInfo.hasNext ? 'disabled' : ''}`}>
          <button className="page-link" onClick={() => onPageChange(paginationInfo.currentPage + 1)} disabled={!paginationInfo.hasNext}>
            Next
          </button>
        </li>
      </ul>
    </nav>
  );
};

export default Pagination;
