'use client';

import React, { useState } from 'react';
import Image from 'next/image';

interface ExchangeItem {
  amount: number;
  currency: string;
  exchangeRate: number;
  lastUpdated: string;
}

const ExchangeItem: React.FC<ExchangeItem> = ({ amount, currency, exchangeRate, lastUpdated }) => {
  const [editableAmount, setEditableAmount] = useState(amount);

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEditableAmount(parseFloat(e.target.value));
  };

  return (
    <div className="flex flex-col h-[245px] items-start p-6 gap-4 bg-white shadow-sm rounded-lg">
      <h2 className="text-lg font-bold">Exchange</h2>
      <div className="flex flex-col w-full">
        <div className="flex items-center gap-2 px-3 border border-gray-300 focus:outline-none rounded-md">
          <input
            type="number"
            value={editableAmount}
            onChange={handleAmountChange}
            className="text-lg font-semibold p-1 rounded-md"
          />
          <Image src="/icon-gbp.png" alt="" width={24} height={24} />
          <span className="text-lg">{currency}</span>
        </div>
        <span className="text-2xl text-center w-full">↔</span>
        <div className="flex items-center gap-2">
          <input
            type="text"
            value={(editableAmount * exchangeRate).toFixed(2)}
            readOnly
            className="text-lg font-semibold border-b border-gray-300 focus:outline-none"
          />
          <Image src="/image-euro.png" alt="" width={24} height={24} />
          <span className="text-lg">EUR</span>
        </div>
      </div>
      <p className="text-xs text-gray-500">
        1 {currency} = {exchangeRate.toFixed(6)} EUR | Mid-market exchange rate at {lastUpdated}
      </p>
    </div>
  );
};

const Exchange: React.FC = () => {
  const exchangeItems: ExchangeItem[] = [
    { amount: 500, currency: 'GBP', exchangeRate: 1.117350, lastUpdated: '13:56 UTC' },
  ];

  return (
    <>
      {exchangeItems.map((item, index) => (
        <ExchangeItem key={index} {...item} />
      ))}
    </>
  );
};

export default Exchange;
