'use client';

import React from 'react';

interface AccountItem {
  status: 'positive' | 'negative';
  type: string;
  iban: string;
  balance: number;
}

const formatBalance = (balance: number): string => {
  return balance.toLocaleString('de-DE', { style: 'currency', currency: 'EUR' });
};

const AccountItem: React.FC<AccountItem> = ({ status, type, iban, balance }) => (
  <div className="flex items-center justify-between w-full">
    <div className="flex items-center gap-2">
      <span className={`w-3 h-3 rounded-full ${status === 'positive' ? 'bg-green-500' : 'bg-red-500'}`}></span>
      <div className="flex flex-col">
        <p className="text-sm font-semibold">{type}</p>
        <p className="text-xs text-gray-500">{iban}</p>
      </div>
    </div>
    <div className="flex items-center gap-2">
      <p className={`text-sm ${status === 'positive' ? 'text-black' : 'text-red-500'}`}>
        {status === 'positive' ? '+' : '-'}{formatBalance(balance)}
      </p>
      <span className="text-gray-500">→</span>
    </div>
  </div>
);

const Accounts: React.FC = () => {
  const accountItems: AccountItem[] = [
    { status: 'positive', type: 'Premium account', iban: 'DE99 5004 0000 1122 3344 55', balance: 124593.00 },
    { status: 'negative', type: 'Premium account', iban: 'DE99 5004 0000 1122 3344 55', balance: -3593.00 },
    { status: 'positive', type: 'Premium account', iban: 'DE99 5004 0000 1122 3344 55', balance: 76293.00 },
  ];

  return (
    <div className="flex flex-col h-[245px] items-start p-6 gap-4 bg-white shadow-sm rounded-lg">
      <div className="flex justify-between items-center w-full">
        <h2 className="text-lg font-bold">Accounts</h2>
        <button className="text-gray-500">•••</button>
      </div>
      {accountItems.map((item, index) => (
        <AccountItem key={index} {...item} />
      ))}
    </div>
  );
};

export default Accounts;
