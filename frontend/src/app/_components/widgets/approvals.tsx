'use client';

import React from 'react';
import Image from 'next/image';

interface ApprovalItem {
  type: string;
  title: string;
  date: string;
  amount: number;
  unit?: string;
  description?: string;
}

const formatNumber = (num: number): string => {
  if (num >= 1000000) {
    return (num / 1000000).toFixed(1) + 'm';
  } else if (num >= 1000) {
    return (num / 1000).toFixed(1) + 'k';
  }
  return num.toString();
};

const ApprovalItem: React.FC<ApprovalItem> = ({ type, title, date, amount, unit, description }) => (
  <div className="flex items-center justify-between w-full">
    <div className="flex items-center gap-1">
      <Image src={`/icon-${type}.svg`} alt="" width={24} height={24} />
      <div className="flex flex-col gap-1">
      <div className="flex gap-1">
        <p className="text-sm font-semibold">{title}</p>
        <p className="text-xs text-gray-500">{date}</p>
      </div>
      <p className="text-xs text-gray-500">{description}</p>
      </div>
    </div>
    <div className="flex gap-2 justify-center items-center">
    <p className="text-xs text-gray-500">{formatNumber(amount)}{unit}</p>
      <button className="bg-yellow-400 text-sm px-3 py-2 rounded h-full">Review→</button>
    </div>
  </div>
);

const Approvals: React.FC = () => {
  const approvalItems: ApprovalItem[] = [
    { type: 'akkreditiv', title: 'New akkreditiv', date: '19/10/2024', amount: 2000000, unit: '€/0.83%', description: 'LC2024051500001' },
    { type: 'credit-card', title: 'New credit card', date: '19/10/2024', amount: 8800, unit: '€/m limit', description: 'For Martina Mustermaier' },
    { type: 'lawn', title: 'New lawn', date: '19/10/2024', amount: 32000, unit: '€/12 m/4,82%', description: 'For IT department' },
  ];

  return (
    <div className="flex flex-col h-[245px] items-start p-6 gap-4 bg-white shadow-sm rounded-lg">
      <div className="flex justify-between items-center w-full">
        <h2 className="text-lg font-bold flex items-center gap-2">
          Requests for approvals
          <span className="bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">3</span>
        </h2>
        <button className="text-gray-500">•••</button>
      </div>
      {approvalItems.map((item, index) => (
        <ApprovalItem key={index} {...item} />
      ))}
    </div>
  );
};

export default Approvals;
